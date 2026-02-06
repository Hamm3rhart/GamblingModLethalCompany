using System;
using System.Collections;
using GamblersMod.Player;
using Unity.Netcode;
using UnityEngine;
using GameNetcodeStuff;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : NetworkBehaviour
    {
        int gamblingMachineMaxCooldown;
        public int gamblingMachineCurrentCooldown = 0;

        float jackpotMultiplier;
        float tripleMultiplier;
        float doubleMultiplier;
        float halvedMultiplier;
        float zeroMultiplier;
        float explodeMultiplier;

        int jackpotPercentage;
        int triplePercentage;
        int doublePercentage;
        int halvedPercentage;
        int removedPercentage;
        int explodePercentage;

        int maxValueLimit;

        bool isMusicEnabled = true;
        float musicVolume = 0.35f;

        int rollMinValue;
        int rollMaxValue;
        int currentRoll = 100;

        public float currentGamblingOutcomeMultiplier = 1;
        public string currentGamblingOutcome = GamblingOutcome.DEFAULT;

        private Coroutine CountdownCooldownCoroutineBeingRan;

        bool lockGamblingMachineServer = false;

        public int numberOfUses;

        void Awake()
        {
            Plugin.mls.LogInfo("GamblingMachine has Awoken");

            gamblingMachineMaxCooldown = Plugin.CurrentUserConfig.configMaxCooldown;
            numberOfUses = Plugin.CurrentUserConfig.configNumberOfUses;

            jackpotMultiplier = Plugin.CurrentUserConfig.configJackpotMultiplier;
            tripleMultiplier = Plugin.CurrentUserConfig.configTripleMultiplier;
            doubleMultiplier = Plugin.CurrentUserConfig.configDoubleMultiplier;
            halvedMultiplier = Plugin.CurrentUserConfig.configHalveMultiplier;
            zeroMultiplier = Plugin.CurrentUserConfig.configZeroMultiplier;
            explodeMultiplier = Plugin.CurrentUserConfig.configExplodeMultiplier;

            jackpotPercentage = Plugin.CurrentUserConfig.configJackpotChance;
            triplePercentage = Plugin.CurrentUserConfig.configTripleChance;
            doublePercentage = Plugin.CurrentUserConfig.configDoubleChance;
            halvedPercentage = Plugin.CurrentUserConfig.configHalveChance;
            removedPercentage = Plugin.CurrentUserConfig.configZeroChance;
            explodePercentage = Plugin.CurrentUserConfig.configExplodeChance;

            maxValueLimit = Plugin.CurrentUserConfig.configMaxValueLimit;

            isMusicEnabled = Plugin.CurrentUserConfig.configGamblingMusicEnabled;
            musicVolume = Plugin.CurrentUserConfig.configGamblingMusicVolume;

            Plugin.mls.LogInfo($"GamblingMachine: gamblingMachineMaxCooldown loaded from config: {gamblingMachineMaxCooldown}");

            Plugin.mls.LogInfo($"GamblingMachine: jackpotMultiplier loaded from config: {jackpotMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: tripleMultiplier loaded from config: {tripleMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: doubleMultiplier loaded from config: {doubleMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: halvedMultiplier loaded from config: {halvedMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: zeroMultiplier loaded from config: {zeroMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: explodeMultiplier loaded from config: {explodeMultiplier}");

            Plugin.mls.LogInfo($"GamblingMachine: jackpotPercentage loaded from config: {jackpotPercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: triplePercentage loaded from config: {triplePercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: doublePercentage loaded from config: {doublePercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: halvedPercentage loaded from config: {halvedPercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: removedPercentage loaded from config: {removedPercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: explodePercentage loaded from config: {explodePercentage}");

            Plugin.mls.LogInfo($"GamblingMachine: maxValueLimit loaded from config: {maxValueLimit}");

            Plugin.mls.LogInfo($"GamblingMachine: gamblingMusicEnabled loaded from config: {isMusicEnabled}");
            Plugin.mls.LogInfo($"GamblingMachine: gamblingMusicVolume loaded from config: {musicVolume}");

            InitAudioSource();

            rollMinValue = 1;
            rollMaxValue = jackpotPercentage + triplePercentage + doublePercentage + halvedPercentage + removedPercentage + explodePercentage;
        }

        void Start()
        {
            Plugin.mls.LogInfo("GamblingMachine has Started");
        }

        public void GenerateGamblingOutcomeFromCurrentRoll()
        {
            bool isJackpotRoll = currentRoll >= rollMinValue && currentRoll <= jackpotPercentage;

            int tripleStart = jackpotPercentage;
            int tripleEnd = jackpotPercentage + triplePercentage;
            bool isTripleRoll = currentRoll > tripleStart && currentRoll <= tripleEnd;

            int doubleStart = tripleEnd;
            int doubleEnd = tripleEnd + doublePercentage;
            bool isDoubleRoll = currentRoll > doubleStart && currentRoll <= doubleEnd;

            int halvedStart = doubleEnd;
            int halvedEnd = doubleEnd + halvedPercentage;
            bool isHalvedRoll = currentRoll > halvedStart && currentRoll <= halvedEnd;

            int explodeStart = halvedEnd;
            int explodeEnd = halvedEnd + explodePercentage;
            bool isExplodeRoll = currentRoll > explodeStart && currentRoll <= explodeEnd;

            if (isJackpotRoll)
            {
                Plugin.mls.LogMessage("Rolled Jackpot");
                currentGamblingOutcomeMultiplier = jackpotMultiplier;
                currentGamblingOutcome = GamblingOutcome.JACKPOT;
            }
            else if (isTripleRoll)
            {
                Plugin.mls.LogMessage("Rolled Triple");
                currentGamblingOutcomeMultiplier = tripleMultiplier;
                currentGamblingOutcome = GamblingOutcome.TRIPLE;
            }
            else if (isDoubleRoll)
            {
                Plugin.mls.LogMessage("Rolled Double");
                currentGamblingOutcomeMultiplier = doubleMultiplier;
                currentGamblingOutcome = GamblingOutcome.DOUBLE;
            }
            else if (isHalvedRoll)
            {
                Plugin.mls.LogMessage("Rolled Halved");
                currentGamblingOutcomeMultiplier = halvedMultiplier;
                currentGamblingOutcome = GamblingOutcome.HALVE;
            }
            else if (isExplodeRoll)
            {
                Plugin.mls.LogMessage("Rolled Explode");
                currentGamblingOutcomeMultiplier = explodeMultiplier;
                currentGamblingOutcome = GamblingOutcome.EXPLODE;
            }
            else
            {
                Plugin.mls.LogMessage("Rolled Remove");
                currentGamblingOutcomeMultiplier = zeroMultiplier;
                currentGamblingOutcome = GamblingOutcome.REMOVE;
            }
        }

        private void SpawnExplosion(Vector3 position)
        {
            // Spawn vanilla landmine explosion (handles SFX/VFX/damage)
            Landmine.SpawnExplosion(position, true, 1f, 1f, 50, 0f, null, false);
        }

        private IEnumerator DelayedExplosion(Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnExplosion(position);
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
            // EXPLODE uses Landmine.SpawnExplosion for SFX/VFX, no direct clip here
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
                StopCoroutine(CountdownCooldownCoroutineBeingRan);
            }
            CountdownCooldownCoroutineBeingRan = StartCoroutine(CountdownCooldownCoroutine(onCountdownFinish));
        }

        public bool isInCooldownPhase()
        {
            return gamblingMachineCurrentCooldown > 0;
        }

        IEnumerator CountdownCooldownCoroutine(Action onCountdownFinish)
        {
            Plugin.mls.LogInfo("Start gambling machine cooldown");
            while (gamblingMachineCurrentCooldown > 0)
            {
                yield return new WaitForSeconds(1);
                gamblingMachineCurrentCooldown -= 1;
                Plugin.mls.LogMessage($"Gambling machine cooldown: {gamblingMachineCurrentCooldown}");
            }
            onCountdownFinish();
            Plugin.mls.LogMessage("End gambling machine cooldown");
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
            int roll = UnityEngine.Random.Range(rollMinValue, rollMaxValue + 1); // upper bound is exclusive, so add 1
            currentRoll = roll;

            Plugin.mls.LogMessage($"rollMinValue: {rollMinValue}");
            Plugin.mls.LogMessage($"rollMaxValue: {rollMaxValue}");
            Plugin.mls.LogMessage($"Rolled value: {roll}");

            return roll;
        }

        public int GetScrapValueBasedOnGambledOutcome(GrabbableObject scrap)
        {
            // Use double to avoid overflow before clamping
            double raw = scrap.scrapValue * (double)currentGamblingOutcomeMultiplier;
            double capped;

            if (maxValueLimit > 0)
            {
                capped = Math.Min(raw, maxValueLimit);
            }
            else
            {
                capped = Math.Min(raw, int.MaxValue);
            }

            return (int)Math.Floor(capped);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ActivateGamblingMachineServerRPC(NetworkBehaviourReference scrapBeingGambledRef, NetworkBehaviourReference playerWhoGambledRef, ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer) return;

            if (numberOfUses <= 0)
            {
                Plugin.mls.LogWarning("ActivateGamblingMachineServerRPC: Machine usage limit has been reached");
                return;
            }

            if (lockGamblingMachineServer)
            {
                Plugin.mls.LogWarning($"Gambling machine is already processing one client's request. Throwing away a request for... {serverRpcParams.Receive.SenderClientId}");
                return;
            }

            lockGamblingMachineServer = true;
            numberOfUses -= 1;
            Plugin.mls.LogInfo($"ActivateGamblingMachineServerRPC: Number of uses left: {numberOfUses}");

            if (!scrapBeingGambledRef.TryGet(out GrabbableObject scrapBeingGambled))
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: Failed to get scrap value on client side.");
                return;
            }

            BeginGamblingMachineCooldownClientRpc();
            Plugin.mls.LogMessage("ActivateGamblingMachineServerRPC: Starting gambling machine cooldown phase in the server invoked by: " + serverRpcParams.Receive.SenderClientId);

            SetRoll(RollDice());
            GenerateGamblingOutcomeFromCurrentRoll();
            int updatedScrapValue = GetScrapValueBasedOnGambledOutcome(scrapBeingGambled);

            ActivateGamblingMachineClientRPC(scrapBeingGambledRef, playerWhoGambledRef, serverRpcParams.Receive.SenderClientId, updatedScrapValue, currentGamblingOutcome, numberOfUses);
        }

        [ClientRpc]
        void ActivateGamblingMachineClientRPC(NetworkBehaviourReference scrapBeingGambledRef, NetworkBehaviourReference playerWhoGambledRef, ulong invokerId, int updatedScrapValue, string outcome, int numberOfUsesServer)
        {
            Plugin.mls.LogInfo("ActivateGamblingMachineClientRPC: Activiating gambling machines on client...");

            numberOfUses = numberOfUsesServer;
            Plugin.mls.LogInfo($"ActivateGamblingMachineClientRPC: Number of uses left: {numberOfUses}");

            if (!playerWhoGambledRef.TryGet(out PlayerControllerCustom playerWhoGambled))
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get player who gambled.");
                return;
            }

            playerWhoGambled.LockGamblingMachine();
            PlayDrumRoll();

            BeginGamblingMachineCooldown(() =>
            {
                if (!scrapBeingGambledRef.TryGet(out GrabbableObject scrapBeingGambled))
                {
                    Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get scrap value on client side.");
                    return;
                }

                Plugin.mls.LogInfo($"Setting scrap value to: {updatedScrapValue}");
                scrapBeingGambled.SetScrapValue(updatedScrapValue);
                PlayGambleResultAudio(outcome);
                if (outcome == GamblingOutcome.EXPLODE)
                {
                    // Play pre-explosion stinger for all clients
                    if (Plugin.GamblingEmotionalDamageAudio)
                    {
                        AudioSource.PlayClipAtPoint(Plugin.GamblingEmotionalDamageAudio, playerWhoGambled.transform.position, 0.7f);
                    }

                    if (IsServer)
                    {
                        StartCoroutine(DelayedExplosion(playerWhoGambled.transform.position, 2f));
                    }
                }
                playerWhoGambled.ReleaseGamblingMachineLock();

                if (IsServer)
                {
                    Plugin.mls.LogMessage("Unlocking gambling machine");
                    lockGamblingMachineServer = false;
                }

            });
        }

        [ClientRpc]
        void BeginGamblingMachineCooldownClientRpc()
        {
            Plugin.mls.LogInfo("Setting machine cooldown to max");
            SetCurrentGamblingCooldownToMaxCooldown();
        }

        private void InitAudioSource()
        {
            if (!isMusicEnabled)
            {
                GetComponent<AudioSource>().Pause();
            }

            GetComponent<AudioSource>().volume = musicVolume;
        }
    }
}
