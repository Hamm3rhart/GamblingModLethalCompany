using System.Linq;
using HarmonyLib;
using Unity.Netcode;

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
}
