using System;
using System.Collections;
using GamblersMod.Player;
using Unity.Netcode;
using UnityEngine;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : NetworkBehaviour
    {
        // Cooldown
        int gamblingMachineMaxCooldown;
        public int gamblingMachineCurrentCooldown = 0;

        // Multipliers for winning or losing
        float jackpotMultiplier;
        float tripleMultiplier;
        float doubleMultiplier;
        float halvedMultiplier;
        float zeroMultiplier;

        // Percentages for the outcome of gambling
        int jackpotPercentage;
        int triplePercentage;
        int doublePercentage;
        int halvedPercentage;
        int removedPercentage;

        // Audio
        bool isMusicEnabled = true;
        float musicVolume = 0.35f;

        // Dice roll range (inclusive)
        int rollMinValue;
        int rollMaxValue;
        int currentRoll = 100;

        // Current state
        public float currentGamblingOutcomeMultiplier = 1;
        public string currentGamblingOutcome = GamblingOutcome.DEFAULT;

        // TODO (Think about this better)
        private Coroutine CountdownCooldownCoroutineBeingRan;

        bool lockGamblingMachineServer = false;

        // Limitations
        public int numberOfUses;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Plugin.LogDebug($"[GambleLayer] OnNetworkSpawn entered isOwner={IsOwner} isServer={IsServer} currentLayer={gameObject.layer}");

            // Align layer/scale and ensure colliders are enabled on clients as well
            gameObject.tag = "Untagged";

            int interactLayer = LayerMask.NameToLayer("InteractableObject");
            if (interactLayer < 0)
            {
                Plugin.LogDebug("[GambleLayer] Layer 'InteractableObject' not found; using current layer");
                interactLayer = gameObject.layer;
            }

            gameObject.layer = interactLayer;
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            var colliders = GetComponentsInChildren<Collider>(includeInactive: true);
            Plugin.LogDebug($"[GambleLayer] OnNetworkSpawn isOwner={IsOwner} isServer={IsServer} layer={gameObject.layer} colliders={colliders.Length}");
            foreach (var col in colliders)
            {
                if (col == null) continue;
                col.enabled = true;
                col.gameObject.layer = interactLayer;
                Plugin.LogDebug($"[GambleCollider] path={GetPath(col.transform)} layer={col.gameObject.layer} enabled={col.enabled} trigger={col.isTrigger} bounds={col.bounds}");
            }
        }

        private static string GetPath(Transform t)
        {
            if (t == null) return "<null>";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Transform current = t;
            while (current != null)
            {
                sb.Insert(0, current.name);
                current = current.parent;
                if (current != null) sb.Insert(0, "/");
            }

            return sb.ToString();
        }

        void Awake()
        {
            Plugin.LogDebug("GamblingMachine has Awoken");

            // Log collider state as early fallback in case OnNetworkSpawn is skipped
            var colliders = GetComponentsInChildren<Collider>(includeInactive: true);
            Plugin.LogDebug($"[GambleLayer] Awake colliders={colliders.Length} rootLayer={gameObject.layer}");
            foreach (var col in colliders)
            {
                if (col == null) continue;
                Plugin.LogDebug($"[GambleCollider] Awake path={GetPath(col.transform)} layer={col.gameObject.layer} enabled={col.enabled} trigger={col.isTrigger} bounds={col.bounds}");
            }

            // General
            gamblingMachineMaxCooldown = Plugin.CurrentUserConfig.configMaxCooldown;
            numberOfUses = Plugin.CurrentUserConfig.configNumberOfUses;

            // Multipliers
            jackpotMultiplier = Plugin.CurrentUserConfig.configJackpotMultiplier;
            tripleMultiplier = Plugin.CurrentUserConfig.configTripleMultiplier;
            doubleMultiplier = Plugin.CurrentUserConfig.configDoubleMultiplier;
            halvedMultiplier = Plugin.CurrentUserConfig.configHalveMultiplier;
            zeroMultiplier = Plugin.CurrentUserConfig.configZeroMultiplier;

            // Chance
            jackpotPercentage = Plugin.CurrentUserConfig.configJackpotChance;
            triplePercentage = Plugin.CurrentUserConfig.configTripleChance;
            doublePercentage = Plugin.CurrentUserConfig.configDoubleChance;
            halvedPercentage = Plugin.CurrentUserConfig.configHalveChance;
            removedPercentage = Plugin.CurrentUserConfig.configZeroChance;

            // Audio 
            isMusicEnabled = Plugin.CurrentUserConfig.configGamblingMusicEnabled;
            musicVolume = Plugin.CurrentUserConfig.configGamblingMusicVolume;

            Plugin.LogDebug($"GamblingMachine: gamblingMachineMaxCooldown loaded from config: {gamblingMachineMaxCooldown}");

            Plugin.LogDebug($"GamblingMachine: jackpotMultiplier loaded from config: {jackpotMultiplier}");
            Plugin.LogDebug($"GamblingMachine: tripleMultiplier loaded from config: {tripleMultiplier}");
            Plugin.LogDebug($"GamblingMachine: doubleMultiplier loaded from config: {doubleMultiplier}");
            Plugin.LogDebug($"GamblingMachine: halvedMultiplier loaded from config: {halvedMultiplier}");
            Plugin.LogDebug($"GamblingMachine: zeroMultiplier loaded from config: {zeroMultiplier}");

            Plugin.LogDebug($"GamblingMachine: jackpotPercentage loaded from config: {jackpotPercentage}");
            Plugin.LogDebug($"GamblingMachine: triplePercentage loaded from config: {triplePercentage}");
            Plugin.LogDebug($"GamblingMachine: doublePercentage loaded from config: {doublePercentage}");
            Plugin.LogDebug($"GamblingMachine: halvedPercentage loaded from config: {halvedPercentage}");
            Plugin.LogDebug($"GamblingMachine: removedPercentage loaded from config: {removedPercentage}");

            Plugin.LogDebug($"GamblingMachine: gamblingMusicEnabled loaded from config: {isMusicEnabled}");
            Plugin.LogDebug($"GamblingMachine: gamblingMusicVolume loaded from config: {musicVolume}");

            InitAudioSource();

            // Rolls
            rollMinValue = 1;
            rollMaxValue = jackpotPercentage + triplePercentage + doublePercentage + halvedPercentage + removedPercentage;
        }

        void Start()
        {
            Plugin.LogDebug("GamblingMachine has Started");
        }

        public void GenerateGamblingOutcomeFromCurrentRoll()
        {
            bool isJackpotRoll = (currentRoll >= rollMinValue && currentRoll <= jackpotPercentage); // [0 - JACKPOT]

            int tripleStart = jackpotPercentage;
            int tripleEnd = jackpotPercentage + triplePercentage;
            bool isTripleRoll = (currentRoll > tripleStart && currentRoll <= tripleEnd); // [JACKPOT - (JACKPOT + TRIPLE)]

            int doubleStart = tripleEnd;
            int doubleEnd = tripleEnd + doublePercentage;
            bool isDoubleRoll = (currentRoll > doubleStart && currentRoll <= doubleEnd); // [(JACKPOT + TRIPLE) - (JACKPOT + TRIPLE + DOUBLE)]

            int halvedStart = doubleEnd;
            int halvedEnd = doubleEnd + halvedPercentage;
            bool isHalvedRoll = (currentRoll > halvedStart && currentRoll <= halvedEnd); // [(JACKPOT + TRIPLE + DOUBLE) - (JACKPOT + TRIPLE + DOUBLE + HALVED)]

            if (isJackpotRoll)
            {
                Plugin.LogDebug("Rolled Jackpot");
                currentGamblingOutcomeMultiplier = jackpotMultiplier;
                currentGamblingOutcome = GamblingOutcome.JACKPOT;
            }
            else if (isTripleRoll)
            {
                Plugin.LogDebug("Rolled Triple");
                currentGamblingOutcomeMultiplier = tripleMultiplier;
                currentGamblingOutcome = GamblingOutcome.TRIPLE;
            }
            else if (isDoubleRoll)
            {
                Plugin.LogDebug("Rolled Double");
                currentGamblingOutcomeMultiplier = doubleMultiplier;
                currentGamblingOutcome = GamblingOutcome.DOUBLE;
            }
            else if (isHalvedRoll)
            {
                Plugin.LogDebug("Rolled Halved");
                currentGamblingOutcomeMultiplier = halvedMultiplier;
                currentGamblingOutcome = GamblingOutcome.HALVE;
            }
            else
            {
                Plugin.LogDebug("Rolled Remove");
                currentGamblingOutcomeMultiplier = zeroMultiplier;
                currentGamblingOutcome = GamblingOutcome.REMOVE;
            }
        }

        public void PlayGambleResultAudio(string outcome)
        {
            if (outcome == GamblingOutcome.JACKPOT)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingJackpotScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.TRIPLE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingTripleScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.DOUBLE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingDoubleScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.HALVE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingHalveScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.REMOVE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingRemoveScrapAudio, transform.position, 0.6f);
            }
        }

        public void PlayDrumRoll()
        {
            AudioSource.PlayClipAtPoint(Plugin.GamblingDrumrollScrapAudio, transform.position, 0.6f);
        }

        public void BeginGamblingMachineCooldown(Action onCountdownFinish)
        {
            SetCurrentGamblingCooldownToMaxCooldown();
            if (CountdownCooldownCoroutineBeingRan != null)
            {
                StopCoroutine(CountdownCooldownCoroutineBeingRan); // Insurance
            }
            CountdownCooldownCoroutineBeingRan = StartCoroutine(CountdownCooldownCoroutine(onCountdownFinish));
        }

        public bool isInCooldownPhase()
        {
            return gamblingMachineCurrentCooldown > 0;
        }

        IEnumerator CountdownCooldownCoroutine(Action onCountdownFinish)
        {
            Plugin.LogDebug("Start gambling machine cooldown");
            while (gamblingMachineCurrentCooldown > 0)
            {
                yield return new WaitForSeconds(1);
                gamblingMachineCurrentCooldown -= 1;
                Plugin.LogDebug($"Gambling machine cooldown: {gamblingMachineCurrentCooldown}");
            }
            onCountdownFinish();
            Plugin.LogDebug("End gambling machine cooldown");
        }

        public void SetCurrentGamblingCooldownToMaxCooldown()
        {
            gamblingMachineCurrentCooldown = gamblingMachineMaxCooldown;
        }

        public void SetRoll(int newRoll)
        {
            currentRoll = newRoll;
        }

        public int RollDice()
        {
            int roll = UnityEngine.Random.Range(rollMinValue, rollMaxValue);

            Plugin.mls.LogMessage($"rollMinValue: {rollMinValue}");
            Plugin.mls.LogMessage($"rollMaxValue: {rollMaxValue}");
            Plugin.mls.LogMessage($"Roll value: {currentRoll}");

            return roll;
        }

        public int GetScrapValueBasedOnGambledOutcome(GrabbableObject scrap)
        {
            return (int)Mathf.Floor(scrap.scrapValue * currentGamblingOutcomeMultiplier);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ActivateGamblingMachineServerRPC(NetworkBehaviourReference scrapBeingGambledRef, NetworkBehaviourReference playerWhoGambledRef, ServerRpcParams serverRpcParams = default)
        {
            Plugin.LogDebug($"[GambleRPC] ActivateGamblingMachineServerRPC sender={serverRpcParams.Receive.SenderClientId} hasScrapRef={scrapBeingGambledRef.IsValid} hasPlayerRef={playerWhoGambledRef.IsValid}");
            if (!IsServer) return;

            // Machine cannot be used anymore
            if (numberOfUses <= 0)
            {
                Plugin.mls.LogWarning("ActivateGamblingMachineServerRPC: Machine usage limit has been reached");
                return;
            };

            // Potentially block multiple requests from processing
            if (lockGamblingMachineServer)
            {
                Plugin.mls.LogWarning($"Gambling machine is already processing one client's request. Throwing away a request for... {serverRpcParams.Receive.SenderClientId}");
                return;
            };

            lockGamblingMachineServer = true; // Lock any further request
            numberOfUses -= 1;
            Plugin.mls.LogInfo($"ActivateGamblingMachineServerRPC: Number of uses left: {numberOfUses}");

            if (!scrapBeingGambledRef.TryGet(out GrabbableObject scrapBeingGambled))
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: Failed to get scrap value on client side.");
                return;
            }

            // Tell all clients to activate their gambling machine cooldown
            BeginGamblingMachineCooldownClientRpc();

            Plugin.mls.LogMessage("ActivateGamblingMachineServerRPC: Starting gambling machine cooldown phase in the server invoked by: " + serverRpcParams.Receive.SenderClientId);

            // Server side logic and send down the final scrap value
            SetRoll(RollDice());
            GenerateGamblingOutcomeFromCurrentRoll();
            int updatedScrapValue = GetScrapValueBasedOnGambledOutcome(scrapBeingGambled);

            // TODO: This function doesn't need to be this big
            ActivateGamblingMachineClientRPC(scrapBeingGambledRef, playerWhoGambledRef, serverRpcParams.Receive.SenderClientId, updatedScrapValue, currentGamblingOutcome, numberOfUses);
        }

        [ClientRpc]
        void ActivateGamblingMachineClientRPC(NetworkBehaviourReference scrapBeingGambledRef, NetworkBehaviourReference playerWhoGambledRef, ulong invokerId, int updatedScrapValue, string outcome, int numberOfUsesServer)
        {
            Plugin.mls.LogInfo("ActivateGamblingMachineClientRPC: Activiating gambling machines on client...");

            // Sync client with server state
            numberOfUses = numberOfUsesServer;
            Plugin.mls.LogInfo($"ActivateGamblingMachineClientRPC: Number of uses left: {numberOfUses}");

            if (!playerWhoGambledRef.TryGet(out PlayerControllerCustom playerWhoGambled))
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get player who gambled.");
                return;
            }

            playerWhoGambled.LockGamblingMachine();
            PlayDrumRoll();

            // Start cooldown for all clients
            BeginGamblingMachineCooldown(() =>
            {
                if (!scrapBeingGambledRef.TryGet(out GrabbableObject scrapBeingGambled))
                {
                    Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get scrap value on client side.");
                    return;
                }

                // Update scrap value for all client
                Plugin.mls.LogInfo($"Setting scrap value to: {updatedScrapValue}");
                scrapBeingGambled.SetScrapValue(updatedScrapValue);
                PlayGambleResultAudio(outcome);
                playerWhoGambled.ReleaseGamblingMachineLock();


                // Server is also a client. We will release the lock on the machine here
                if (IsServer)
                {
                    Plugin.mls.LogMessage("Unlocking gambling machine");
                    lockGamblingMachineServer = false;
                }

            });
        }

        // Set cooldown for all clients, not counting down yet until the server says so
        [ClientRpc]
        void BeginGamblingMachineCooldownClientRpc()
        {
            Plugin.mls.LogInfo("Setting machine cooldown to max");
            SetCurrentGamblingCooldownToMaxCooldown();
        }

        private void InitAudioSource()
        {
            // Music by default plays on awake
            if (!isMusicEnabled)
            {
                GetComponent<AudioSource>().Pause();
            }

            GetComponent<AudioSource>().volume = musicVolume;
        }
    }
}
