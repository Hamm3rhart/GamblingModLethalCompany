using System;
using GamblersMod.config;
using GamblersMod.Player;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;

namespace GamblersMod.Patches
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(PlayerControllerB))]
	internal class PlayerControllerBPatch
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00004494 File Offset: 0x00002694
		[HarmonyPatch("Awake")]
		[HarmonyPrefix]
		public static void Awake(PlayerControllerB __instance)
		{
			__instance.gameObject.AddComponent<PlayerControllerCustom>();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000044A4 File Offset: 0x000026A4
		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
		[HarmonyPostfix]
		public static void ConnectClientToPlayerObjectPatch()
		{
			Plugin.mls.LogInfo("ConnectClientToPlayerObjectPatch");
			bool isHost = NetworkManager.Singleton.IsHost;
			if (isHost)
			{
				Plugin.mls.LogInfo("Registering host config message handler: Hamm3rhart.GamblersMod_" + GambleConstants.ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST);
				NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("Hamm3rhart.GamblersMod_" + GambleConstants.ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST, new CustomMessagingManager.HandleNamedMessageDelegate(GambleConfigNetworkHelper.OnHostRecievesClientConfigRequest));
			}
			else
			{
				NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("Hamm3rhart.GamblersMod_" + GambleConstants.ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST, new CustomMessagingManager.HandleNamedMessageDelegate(GambleConfigNetworkHelper.OnClientRecievesHostConfigRequest));
				GambleConfigNetworkHelper.StartClientRequestConfigFromHost();
			}
		}
	}
}
