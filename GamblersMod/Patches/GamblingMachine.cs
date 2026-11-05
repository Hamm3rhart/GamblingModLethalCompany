using System;
using System.Collections;
using System.Collections.Generic;
using GamblersMod.Player;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using GameNetcodeStuff;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : NetworkBehaviour
    {
        private static readonly List<GamblingMachine> ActiveMachines = new List<GamblingMachine>();
        private static GameObject MusicEmitter;
        private static AudioSource MusicSource;

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

        private int lastResultNonce = int.MinValue;

        public int numberOfUses;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            RegisterMachine();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            UnregisterMachine();
        }

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
            FixPlaceholderShaders();

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

        // Runs on EVERY client: Landmine.SpawnExplosion is purely local (it spawns the VFX/SFX on this
        // machine and only damages the local player), so each client has to trigger it itself.
        private IEnumerator DelayedExplosion(PlayerControllerCustom playerWhoGambled, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (playerWhoGambled == null)
            {
                yield break;
            }

            // Read the position after the delay so the blast happens where the player is now
            SpawnExplosion(playerWhoGambled.transform.position);

            // Only the owning client may kill its own player (KillPlayer returns early otherwise).
            // KillPlayer then syncs death + body to everyone. This also catches players who walked
            // out of the 1m kill radius during the stinger.
            PlayerControllerB controller = playerWhoGambled.OriginalController;
            if (controller != null && controller == GameNetworkManager.Instance.localPlayerController && !controller.isPlayerDead)
            {
                playerWhoGambled.KillWithExplosion();
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
        public void ActivateGamblingMachineServerRPC(NetworkObjectReference scrapBeingGambledRef, NetworkObjectReference playerWhoGambledRef, ServerRpcParams serverRpcParams = default)
        {
            Plugin.LogDebug($"[GambleRPC] ActivateGamblingMachineServerRPC sender={serverRpcParams.Receive.SenderClientId}");
            if (!IsServer) return;

            if (!scrapBeingGambledRef.TryGet(out NetworkObject scrapObj) || scrapObj == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: Failed to get scrap object on server.");
                return;
            }

            if (!playerWhoGambledRef.TryGet(out NetworkObject playerObj) || playerObj == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: Failed to get player object on server.");
                return;
            }

            var scrapBeingGambled = scrapObj.GetComponent<GrabbableObject>();
            if (scrapBeingGambled == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: GrabbableObject missing on scrap object.");
                return;
            }

            var playerWhoGambled = playerObj.GetComponent<Player.PlayerControllerCustom>();
            if (playerWhoGambled == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: PlayerControllerCustom missing on player object.");
                return;
            }

            ProcessGambleRequest(scrapBeingGambled, playerWhoGambled, serverRpcParams.Receive.SenderClientId);
        }

        internal void ProcessGambleRequest(GrabbableObject scrapBeingGambled, Player.PlayerControllerCustom playerWhoGambled, ulong senderClientId)
        {
            if (!IsServer)
            {
                Plugin.LogDebug("[GambleRPC] ProcessGambleRequest called on non-server; ignoring");
                return;
            }

            if (numberOfUses <= 0)
            {
                Plugin.mls.LogWarning("ProcessGambleRequest: Machine usage limit has been reached");
                return;
            }

            if (lockGamblingMachineServer)
            {
                Plugin.mls.LogWarning($"Gambling machine is already processing one client's request. Throwing away a request for... {senderClientId}");
                return;
            }

            lockGamblingMachineServer = true;
            numberOfUses -= 1;
            Plugin.mls.LogInfo($"ProcessGambleRequest: Number of uses left: {numberOfUses}");

            BeginGamblingMachineCooldownClientRpc();
            Plugin.mls.LogMessage("ProcessGambleRequest: Starting gambling machine cooldown phase in the server invoked by: " + senderClientId);

            SetRoll(RollDice());
            GenerateGamblingOutcomeFromCurrentRoll();
            int updatedScrapValue = GetScrapValueBasedOnGambledOutcome(scrapBeingGambled);
            int resultNonce = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            // Always send ClientRPC (works reliably for MAX)
            ActivateGamblingMachineClientRPC(new NetworkObjectReference(scrapBeingGambled.NetworkObject), new NetworkObjectReference(playerWhoGambled.NetworkObject), senderClientId, updatedScrapValue, currentGamblingOutcome, numberOfUses, resultNonce);

            // Also send relay to all non-host clients (fallback for AUTO)
            var nm = NetworkManager.Singleton;
            if (nm != null && nm.CustomMessagingManager != null)
            {
                foreach (var clientId in nm.ConnectedClientsIds)
                {
                    if (clientId == NetworkManager.ServerClientId)
                    {
                        continue;
                    }

                    using (var writer = new FastBufferWriter(sizeof(ulong) * 3 + sizeof(int) * 3 + 64, Allocator.Temp))
                    {
                        writer.WriteValueSafe(NetworkObjectId);
                        writer.WriteValueSafe(scrapBeingGambled.NetworkObjectId);
                        writer.WriteValueSafe(playerWhoGambled.NetworkObjectId);
                        writer.WriteValueSafe(updatedScrapValue);
                        writer.WriteValueSafe(numberOfUses);
                        writer.WriteValueSafe(resultNonce);
                        writer.WriteValueSafe(new FixedString32Bytes(currentGamblingOutcome));
                        nm.CustomMessagingManager.SendNamedMessage(GambleResultRelay.MessageName, clientId, writer);
                    }
                }
            }
        }

        internal void HandleGambleResult(GrabbableObject scrapBeingGambled, PlayerControllerCustom playerWhoGambled, int updatedScrapValue, string outcome, int numberOfUsesServer, int resultNonce)
        {
            if (lastResultNonce == resultNonce)
            {
                Plugin.LogDebug("HandleGambleResult: Duplicate result ignored");
                return;
            }

            lastResultNonce = resultNonce;
            Plugin.mls.LogInfo("HandleGambleResult: Applying gamble result on client...");

            numberOfUses = numberOfUsesServer;
            Plugin.mls.LogInfo($"HandleGambleResult: Number of uses left: {numberOfUses}");

            playerWhoGambled.LockGamblingMachine();
            PlayDrumRoll();

            BeginGamblingMachineCooldown(() =>
            {
                Plugin.mls.LogInfo($"Setting scrap value to: {updatedScrapValue}");
                scrapBeingGambled.SetScrapValue(updatedScrapValue);
                PlayGambleResultAudio(outcome);
                if (outcome == GamblingOutcome.EXPLODE)
                {
                    if (Plugin.GamblingEmotionalDamageAudio)
                    {
                        AudioSource.PlayClipAtPoint(Plugin.GamblingEmotionalDamageAudio, playerWhoGambled.transform.position, 0.7f);
                    }

                    // Not gated by IsServer: every client spawns its own explosion (see DelayedExplosion)
                    StartCoroutine(DelayedExplosion(playerWhoGambled, 2f));
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
        void ActivateGamblingMachineClientRPC(NetworkObjectReference scrapBeingGambledRef, NetworkObjectReference playerWhoGambledRef, ulong invokerId, int updatedScrapValue, string outcome, int numberOfUsesServer, int resultNonce)
        {
            Plugin.mls.LogInfo("ActivateGamblingMachineClientRPC: Activiating gambling machines on client...");

            numberOfUses = numberOfUsesServer;
            Plugin.mls.LogInfo($"ActivateGamblingMachineClientRPC: Number of uses left: {numberOfUses}");

            if (!playerWhoGambledRef.TryGet(out NetworkObject playerObj) || playerObj == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get player object.");
                return;
            }

            var playerWhoGambled = playerObj.GetComponent<PlayerControllerCustom>();
            if (playerWhoGambled == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: PlayerControllerCustom missing on player object.");
                return;
            }

            if (!scrapBeingGambledRef.TryGet(out NetworkObject scrapObj) || scrapObj == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get scrap object on client.");
                return;
            }

            var scrapBeingGambled = scrapObj.GetComponent<GrabbableObject>();
            if (scrapBeingGambled == null)
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: GrabbableObject missing on scrap object.");
                return;
            }

            HandleGambleResult(scrapBeingGambled, playerWhoGambled, updatedScrapValue, outcome, numberOfUsesServer, resultNonce);
        }

        [ClientRpc]
        void BeginGamblingMachineCooldownClientRpc()
        {
            Plugin.mls.LogInfo("Setting machine cooldown to max");
            SetCurrentGamblingCooldownToMaxCooldown();
        }

        private void InitAudioSource()
        {
            var source = GetComponent<AudioSource>();
            if (source == null)
            {
                return;
            }

            // Disable per-machine looping music; use centralized emitter instead
            source.Stop();
            source.enabled = false;
        }

        private static Shader gameLitShader;

        // Workaround until the asset bundle is rebuilt: the current bundle ships a placeholder "HDRP/Lit"
        // (1 pass, no HDRP passes) that HDRP cannot render, so the machine looks transparent.
        // The material data itself is valid HDRP/Lit data, so swapping in the game's real shader is enough.
        private void FixPlaceholderShaders()
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null || material.shader == null)
                    {
                        continue;
                    }

                    if (material.shader.name != "HDRP/Lit" || material.shader.passCount > 1)
                    {
                        continue;
                    }

                    Shader replacement = FindGameLitShader();
                    if (replacement == null)
                    {
                        Plugin.mls.LogWarning("FixPlaceholderShaders: game's HDRP/Lit shader not found; machine may render transparent");
                        return;
                    }

                    material.shader = replacement;
                    Plugin.LogDebug($"FixPlaceholderShaders: replaced placeholder shader on material {material.name}");
                }
            }
        }

        private static Shader FindGameLitShader()
        {
            if (gameLitShader != null)
            {
                return gameLitShader;
            }

            foreach (var shader in Resources.FindObjectsOfTypeAll<Shader>())
            {
                // The bundle's placeholder has the same name but only a single pass
                if (shader != null && shader.name == "HDRP/Lit" && shader.passCount > 1)
                {
                    gameLitShader = shader;
                    break;
                }
            }

            return gameLitShader;
        }

        private void RegisterMachine()
        {
            if (!ActiveMachines.Contains(this))
            {
                ActiveMachines.Add(this);
            }

            UpdateCentralMusicEmitter();
        }

        private void UnregisterMachine()
        {
            ActiveMachines.Remove(this);
            UpdateCentralMusicEmitter();
        }

        private static void UpdateCentralMusicEmitter()
        {
            if (ActiveMachines.Count == 0)
            {
                if (MusicEmitter != null)
                {
                    UnityEngine.Object.Destroy(MusicEmitter);
                    MusicEmitter = null;
                    MusicSource = null;
                }
                return;
            }

            if (!Plugin.CurrentUserConfig.configGamblingMusicEnabled)
            {
                if (MusicEmitter != null)
                {
                    UnityEngine.Object.Destroy(MusicEmitter);
                    MusicEmitter = null;
                    MusicSource = null;
                }
                return;
            }

            Vector3 min = ActiveMachines[0].transform.position;
            Vector3 max = ActiveMachines[0].transform.position;
            foreach (var machine in ActiveMachines)
            {
                var pos = machine.transform.position;
                min = Vector3.Min(min, pos);
                max = Vector3.Max(max, pos);
            }

            Vector3 center = (min + max) * 0.5f;

            if (MusicEmitter == null)
            {
                MusicEmitter = new GameObject("GamblingMachineMusicEmitter");
                MusicSource = MusicEmitter.AddComponent<AudioSource>();
                MusicSource.clip = Plugin.GamblingMachineMusicAudio;
                MusicSource.loop = true;
                MusicSource.spatialBlend = 1f;
                MusicSource.playOnAwake = false;
                MusicSource.rolloffMode = AudioRolloffMode.Linear;
            }

            MusicEmitter.transform.position = center;
            MusicSource.volume = Plugin.CurrentUserConfig.configGamblingMusicVolume;

            if (MusicSource.clip != null && !MusicSource.isPlaying)
            {
                MusicSource.Play();
            }
        }
    }
}
