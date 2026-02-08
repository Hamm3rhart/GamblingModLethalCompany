using GamblersMod.Patches;
using GameNetcodeStuff;
using System.Text;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamblersMod.Player
{
    internal class PlayerControllerCustom : NetworkBehaviour
    {
        private PlayerGamblingUIManager PlayerGamblingUIManager;
        private PlayerControllerB PlayerControllerOriginal;
        public bool isUsingGamblingMachine;

        private float lastRayLogTime;

        public PlayerControllerB OriginalController => PlayerControllerOriginal;

        private void Awake()
        {
            PlayerGamblingUIManager = gameObject.AddComponent<PlayerGamblingUIManager>();
            PlayerControllerOriginal = gameObject.GetComponent<PlayerControllerB>();
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            Camera gameplayCamera = PlayerControllerOriginal.gameplayCamera;
            Vector3 playerPosition = gameplayCamera.transform.position;
            Vector3 forwardDirection = gameplayCamera.transform.forward;
            Ray interactionRay = new Ray(playerPosition, forwardDirection);
            float interactionRayLength = 5f;

            // Use InteractableObject layer mask, fallback to default if missing
            int interactLayer = LayerMask.NameToLayer("InteractableObject");
            int interactableMask = interactLayer >= 0 ? (1 << interactLayer) : Physics.DefaultRaycastLayers;

            if (Physics.Raycast(interactionRay, out RaycastHit interactionRayHit, interactionRayLength, interactableMask) && interactionRayHit.collider)
            {
                if (Time.time - lastRayLogTime > 1f)
                {
                    lastRayLogTime = Time.time;
                    Plugin.LogDebug($"[GambleRay] hit={interactionRayHit.collider.name} layer={interactionRayHit.collider.gameObject.layer} mask={interactableMask} origin={playerPosition} dir={forwardDirection} path={GetPath(interactionRayHit.collider.transform)}");
                }
                var gamblingMachine = interactionRayHit.collider.GetComponentInParent<GamblingMachine>();

                if (gamblingMachine != null)
                {
                    PlayerGamblingUIManager.ShowInteractionText();

                    GrabbableObject heldObject = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];

                    if (gamblingMachine.isInCooldownPhase())
                    {
                        PlayerGamblingUIManager.SetInteractionText($"Cooling down... {gamblingMachine.gamblingMachineCurrentCooldown}");
                    }
                    else if (gamblingMachine.numberOfUses <= 0)
                    {
                        PlayerGamblingUIManager.SetInteractionText("This machine is all used up");
                    }
                    else
                    {
                        string interactKeyName = InputActionRebindingExtensions.GetBindingDisplayString(IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact", false), 0, 0);
                        PlayerGamblingUIManager.SetInteractionText(isUsingGamblingMachine ? "You're already using a machine" : $"Gamble: [{interactKeyName}]");
                    }

                    if (heldObject)
                    {
                        PlayerGamblingUIManager.SetInteractionSubText($"Scrap value on hand: ■{heldObject.scrapValue}");
                    }
                    else
                    {
                        PlayerGamblingUIManager.SetInteractionSubText("Please hold a scrap on your hand");
                    }
                }

                if (gamblingMachine != null && IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact", false).triggered)
                {
                    Plugin.LogDebug($"[GambleRay] interact pressed; machine path={GetPath(gamblingMachine.transform)} netId={gamblingMachine.NetworkObject?.NetworkObjectId}");
                    HandleGamblingMachineInput(gamblingMachine);
                }
            }
            else
            {
                PlayerGamblingUIManager.HideInteractionText();
            }
        }

        private static string GetPath(Transform t)
        {
            if (t == null) return "<null>";
            StringBuilder sb = new StringBuilder();
            Transform current = t;
            while (current != null)
            {
                sb.Insert(0, current.name);
                current = current.parent;
                if (current != null) sb.Insert(0, "/");
            }

            return sb.ToString();
        }

        public void ReleaseGamblingMachineLock()
        {
            Plugin.LogDebug($"Releasing gambling machine lock for: {OwnerClientId}");
            isUsingGamblingMachine = false;
        }

        public void LockGamblingMachine()
        {
            Plugin.LogDebug($"Locking gambling machine for: {OwnerClientId}");
            isUsingGamblingMachine = true;
        }

        private void HandleGamblingMachineInput(GamblingMachine gamblingMachine)
        {
            GrabbableObject heldObject = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];

            if (!heldObject)
            {
                Plugin.LogDebug("[GambleRay] interact ignored: no held object");
                return;
            }

            if (gamblingMachine.isInCooldownPhase() || gamblingMachine.numberOfUses <= 0 || isUsingGamblingMachine)
            {
                Plugin.LogDebug($"[GambleRay] interact ignored: cooldown={gamblingMachine.isInCooldownPhase()} uses={gamblingMachine.numberOfUses} isUsing={isUsingGamblingMachine}");
                return;
            }

            Plugin.LogDebug($"Gambling machine was interacted with by: {PlayerControllerOriginal.playerUsername}");
            gamblingMachine.SetCurrentGamblingCooldownToMaxCooldown();
            Plugin.LogDebug($"Scrap value of {heldObject.name} on hand: ▊{heldObject.scrapValue}");

            if (heldObject.NetworkObject == null || PlayerControllerOriginal.NetworkObject == null)
            {
                Plugin.LogDebug("[GambleRay] interact ignored: missing NetworkObject for held item or player");
                return;
            }

            gamblingMachine.ActivateGamblingMachineServerRPC(new NetworkObjectReference(heldObject.NetworkObject), new NetworkObjectReference(PlayerControllerOriginal.NetworkObject));

            // Fallback delivery via CustomMessagingManager in case the ServerRpc is dropped
            var nm = NetworkManager.Singleton;
            if (nm != null && nm.CustomMessagingManager != null)
            {
                using (var writer = new FastBufferWriter(sizeof(ulong) * 3, Allocator.Temp))
                {
                    writer.WriteValueSafe(gamblingMachine.NetworkObjectId);
                    writer.WriteValueSafe(heldObject.NetworkObjectId);
                    writer.WriteValueSafe(NetworkObjectId);
                    nm.CustomMessagingManager.SendNamedMessage(GambleRequestRelay.MessageName, NetworkManager.ServerClientId, writer);
                }
                Plugin.LogDebug($"[GambleRPC] Sent fallback gamble request msg to server (machineId={gamblingMachine.NetworkObjectId}, scrapId={heldObject.NetworkObjectId}, playerId={NetworkObjectId})");
            }

            PlayerGamblingUIManager.SetInteractionText($"Cooling down... {gamblingMachine.gamblingMachineCurrentCooldown}");
        }

        public void KillWithExplosion()
        {
            // Use standard blast cause to mirror landmine behavior
            PlayerControllerOriginal.KillPlayer(Vector3.zero, true, CauseOfDeath.Blast, 0);
        }
    }
}
