using System;
using HarmonyLib;
using Unity.Netcode;

namespace GamblersMod.Patches
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(GameNetworkManager))]
	internal class GameNetworkManagerPatch
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002FEB File Offset: 0x000011EB
		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		public static void StartPatch(GameNetworkManager __instance)
		{
			Plugin.mls.LogInfo("Adding Gambling machine to network prefab");
			NetworkManager.Singleton.AddNetworkPrefab(Plugin.GamblingMachine);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000300E File Offset: 0x0000120E
		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
		public static void StartDisconnectPatch()
		{
			Plugin.mls.LogInfo("Player disconnected. Resetting the user's configuration settings.");
			Plugin.CurrentUserConfig = Plugin.UserConfigSnapshot;
			GamblingMachineManager.Instance.Reset();
		}
	}
}
