using System.Linq;
using HarmonyLib;
using Unity.Netcode;
using Unity.Collections;
using GamblersMod.Player;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix(GameNetworkManager __instance)
        {
            if (Plugin.GamblingMachine == null || NetworkManager.Singleton == null)
            {
                Plugin.mls.LogError("Cannot register gambling machine prefab: missing asset or NetworkManager");
                return;
            }

            // Register before the host/client starts so spawn packets are recognized by all peers
            var prefabs = NetworkManager.Singleton.NetworkConfig.Prefabs;
            bool alreadyRegistered = prefabs.Prefabs.Any(p => p.Prefab != null && p.Prefab.name == Plugin.GamblingMachine.name);
            if (!alreadyRegistered)
            {
                Plugin.mls.LogInfo("Registering gambling machine network prefab early");
                NetworkManager.Singleton.AddNetworkPrefab(Plugin.GamblingMachine);
            }

            // Register immediately if possible, and retry once the server is ready
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            GambleRequestRelay.Register();
            GambleResultRelay.Register();
        }

        private static void OnServerStarted()
        {
            GambleRequestRelay.Register();
            GambleResultRelay.Register();
        }

        private static void OnClientConnected(ulong clientId)
        {
            GambleResultRelay.Register();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void StartDisconnectPatch()
        {
            Plugin.mls.LogInfo("Player disconnected. Resetting the user's configuration settings.");
            Plugin.CurrentUserConfig = Plugin.UserConfigSnapshot; // Reset the user's configuration settings
            GamblingMachineManager.Instance.Reset();
        }
    }

    internal static class GambleRequestRelay
    {
        internal const string MessageName = "GamblersMod.GambleRequest";
        private static bool _registered;

        internal static void Register()
        {
            if (_registered) return;
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            if (NetworkManager.Singleton.CustomMessagingManager == null)
            {
                Plugin.LogDebug("[GambleRPC] CustomMessagingManager not ready; skipping relay registration");
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                Plugin.LogDebug("[GambleRPC] Not server; skipping relay registration");
                return;
            }

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, HandleMessage);
            Plugin.LogDebug("[GambleRPC] Registered GambleRequest relay");
            _registered = true;
        }

        private static void HandleMessage(ulong senderClientId, FastBufferReader reader)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (!reader.TryBeginRead(sizeof(ulong) * 3))
            {
                Plugin.LogDebug("[GambleRPC] Received malformed gamble request (not enough data)");
                return;
            }

            reader.ReadValueSafe(out ulong machineId);
            reader.ReadValueSafe(out ulong scrapId);
            reader.ReadValueSafe(out ulong playerId);

            var spawned = NetworkManager.Singleton.SpawnManager.SpawnedObjects;

            if (!spawned.TryGetValue(machineId, out var machineObj))
            {
                Plugin.LogDebug($"[GambleRPC] Machine not found for id={machineId}");
                return;
            }

            var machine = machineObj.GetComponent<GamblingMachine>();
            if (machine == null)
            {
                Plugin.LogDebug($"[GambleRPC] Machine component missing for id={machineId}");
                return;
            }

            spawned.TryGetValue(scrapId, out var scrapObj);
            spawned.TryGetValue(playerId, out var playerObj);

            var scrap = scrapObj ? scrapObj.GetComponent<GrabbableObject>() : null;
            var player = playerObj ? playerObj.GetComponent<Player.PlayerControllerCustom>() : null;

            if (scrap == null)
            {
                Plugin.LogDebug($"[GambleRPC] Scrap not found for id={scrapId}");
                return;
            }

            if (player == null)
            {
                Plugin.LogDebug($"[GambleRPC] Player not found for id={playerId}");
                return;
            }

            machine.ProcessGambleRequest(scrap, player, senderClientId);
        }
    }

    internal static class GambleResultRelay
    {
        internal const string MessageName = "GamblersMod.GambleResult";
        private static bool _registered;

        internal static void Register()
        {
            if (_registered) return;
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            if (NetworkManager.Singleton.CustomMessagingManager == null)
            {
                Plugin.LogDebug("[GambleRPC] CustomMessagingManager not ready; skipping result relay registration");
                return;
            }

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, HandleMessage);
            Plugin.LogDebug("[GambleRPC] Registered GambleResult relay");
            _registered = true;
        }

        private static void HandleMessage(ulong senderClientId, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(sizeof(ulong) * 3 + sizeof(int) * 3))
            {
                Plugin.LogDebug("[GambleRPC] Received malformed gamble result (not enough data)");
                return;
            }

            reader.ReadValueSafe(out ulong machineId);
            reader.ReadValueSafe(out ulong scrapId);
            reader.ReadValueSafe(out ulong playerId);
            reader.ReadValueSafe(out int updatedScrapValue);
            reader.ReadValueSafe(out int numberOfUses);
            reader.ReadValueSafe(out int resultNonce);

            FixedString32Bytes outcome;
            reader.ReadValueSafe(out outcome);

            var spawned = NetworkManager.Singleton.SpawnManager.SpawnedObjects;

            if (!spawned.TryGetValue(machineId, out var machineObj))
            {
                Plugin.LogDebug($"[GambleRPC] Machine not found for id={machineId}");
                return;
            }

            var machine = machineObj.GetComponent<GamblingMachine>();
            if (machine == null)
            {
                Plugin.LogDebug($"[GambleRPC] Machine component missing for id={machineId}");
                return;
            }

            spawned.TryGetValue(scrapId, out var scrapObj);
            spawned.TryGetValue(playerId, out var playerObj);

            var scrap = scrapObj ? scrapObj.GetComponent<GrabbableObject>() : null;
            var player = playerObj ? playerObj.GetComponent<PlayerControllerCustom>() : null;

            if (scrap == null)
            {
                Plugin.LogDebug($"[GambleRPC] Scrap not found for id={scrapId}");
                return;
            }

            if (player == null)
            {
                Plugin.LogDebug($"[GambleRPC] Player not found for id={playerId}");
                return;
            }

            machine.HandleGambleResult(scrap, player, updatedScrapValue, outcome.ToString(), numberOfUses, resultNonce);
        }
    }
}
