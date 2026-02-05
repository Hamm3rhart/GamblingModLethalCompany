using GamblersMod.Patches;
using GameNetcodeStuff;
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
            int interactableMask = 1 << 9;

            if (Physics.Raycast(interactionRay, out RaycastHit interactionRayHit, interactionRayLength, interactableMask) && interactionRayHit.collider)
            {
                GameObject hitObject = interactionRayHit.transform.gameObject;

                if (hitObject.name.Contains("GamblingMachine"))
                {
                    PlayerGamblingUIManager.ShowInteractionText();

                    GrabbableObject heldObject = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];
                    GamblingMachine gamblingMachine = hitObject.GetComponent<GamblingMachine>();

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

                if (hitObject.name.Contains("GamblingMachine") && IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact", false).triggered)
                {
                    GamblingMachine gamblingMachine = hitObject.GetComponent<GamblingMachine>();
                    HandleGamblingMachineInput(gamblingMachine);
                }
            }
            else
            {
                PlayerGamblingUIManager.HideInteractionText();
            }
        }

        public void ReleaseGamblingMachineLock()
        {
            Plugin.mls.LogInfo($"Releasing gambling machine lock for: {OwnerClientId}");
            isUsingGamblingMachine = false;
        }

        public void LockGamblingMachine()
        {
            Plugin.mls.LogInfo($"Locking gambling machine for: {OwnerClientId}");
            isUsingGamblingMachine = true;
        }

        private void HandleGamblingMachineInput(GamblingMachine gamblingMachine)
        {
            GrabbableObject heldObject = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];

            if (!heldObject)
            {
                return;
            }

            if (gamblingMachine.isInCooldownPhase() || gamblingMachine.numberOfUses <= 0 || isUsingGamblingMachine)
            {
                return;
            }

            Plugin.mls.LogInfo($"Gambling machine was interacted with by: {PlayerControllerOriginal.playerUsername}");
            gamblingMachine.SetCurrentGamblingCooldownToMaxCooldown();
            Plugin.mls.LogMessage($"Scrap value of {heldObject.name} on hand: ▊{heldObject.scrapValue}");

            gamblingMachine.ActivateGamblingMachineServerRPC(heldObject, this);
            PlayerGamblingUIManager.SetInteractionText($"Cooling down... {gamblingMachine.gamblingMachineCurrentCooldown}");
        }

        public void KillWithExplosion()
        {
            // Use standard blast cause to mirror landmine behavior
            PlayerControllerOriginal.KillPlayer(Vector3.zero, true, CauseOfDeath.Blast, 0);
        }
    }
}
