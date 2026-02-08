using System;
using BepInEx.Logging;
using Unity.Netcode;

namespace GamblersMod.config
{
	// Token: 0x02000011 RID: 17
	public class GambleConfigNetworkHelper
	{
		// Token: 0x0600005B RID: 91 RVA: 0x0000460C File Offset: 0x0000280C
		public static void OnHostRecievesClientConfigRequest(ulong clientId, FastBufferReader _)
		{
			bool flag = !NetworkManager.Singleton.IsHost;
			if (!flag)
			{
				Plugin.mls.LogInfo("Host recieved client config request.");
				Plugin.mls.LogInfo("Serializing host config data...");
				byte[] serializedSettings = SerializerHelper.GetSerializedSettings<GambleConfigSettingsSerializable>(Plugin.CurrentUserConfig);
				Plugin.mls.LogInfo("Start writing host config data...");
				FastBufferWriter fastBufferWriter;
				fastBufferWriter..ctor(serializedSettings.Length + 4, 2, -1);
				using (fastBufferWriter)
				{
					Plugin.mls.LogInfo("Writing host config data");
					int num = serializedSettings.Length;
					fastBufferWriter.WriteValueSafe<int>(ref num, default(FastBufferWriter.ForPrimitives));
					fastBufferWriter.WriteBytesSafe(serializedSettings, -1, 0);
					Plugin.mls.LogInfo(string.Format("Sending host config data to client with id of {0}...", clientId));
					NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("Hamm3rhart.GamblersMod_" + GambleConstants.ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST, clientId, fastBufferWriter, 4);
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004710 File Offset: 0x00002910
		public static void StartClientRequestConfigFromHost()
		{
			bool flag = !NetworkManager.Singleton.IsClient;
			if (!flag)
			{
				Plugin.mls.LogInfo("Client is requesting configuration from host");
				FastBufferWriter fastBufferWriter;
				fastBufferWriter..ctor(4, 2, -1);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("Hamm3rhart.GamblersMod_" + GambleConstants.ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST, 0UL, fastBufferWriter, 3);
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004770 File Offset: 0x00002970
		public static void OnClientRecievesHostConfigRequest(ulong _, FastBufferReader reader)
		{
			Plugin.mls.LogInfo("Client recieved configuration message from host");
			bool flag = !reader.TryBeginRead(4);
			if (flag)
			{
				Plugin.mls.LogError("Could not sync client configuration with host. The stream sent by StartClientRequestConfigFromHost was invalid.");
			}
			else
			{
				int num;
				reader.ReadValueSafe<int>(ref num, default(FastBufferWriter.ForPrimitives));
				bool flag2 = !reader.TryBeginRead(num);
				if (flag2)
				{
					Plugin.mls.LogError("Could not sync client configuration with host. Host could not serialize the data.");
				}
				byte[] array = new byte[num];
				reader.ReadBytesSafe(ref array, num, 0);
				Plugin.RecentHostConfig = SerializerHelper.GetDeserializedSettings<GambleConfigSettingsSerializable>(array);
				Plugin.CurrentUserConfig = Plugin.RecentHostConfig;
				Plugin.CurrentUserConfig.configGamblingMusicEnabled = Plugin.UserConfigSnapshot.configGamblingMusicEnabled;
				Plugin.CurrentUserConfig.configGamblingMusicVolume = Plugin.UserConfigSnapshot.configGamblingMusicVolume;
				ManualLogSource mls = Plugin.mls;
				mls.LogInfo(string.Format("Cooldown value from config: {0}", Plugin.CurrentUserConfig.configMaxCooldown));
				mls.LogInfo(string.Format("Jackpot chance value from config: {0}", Plugin.CurrentUserConfig.configJackpotChance));
				mls.LogInfo(string.Format("Triple chance value from config: {0}", Plugin.CurrentUserConfig.configTripleChance));
				mls.LogInfo(string.Format("Double chance value from config: {0}", Plugin.CurrentUserConfig.configDoubleChance));
				mls.LogInfo(string.Format("Halve chance value from config: {0}", Plugin.CurrentUserConfig.configHalveChance));
				mls.LogInfo(string.Format("Zero chance value from config: {0}", Plugin.CurrentUserConfig.configZeroChance));
				mls.LogInfo(string.Format("Jackpot multiplier value from config: {0}", Plugin.CurrentUserConfig.configJackpotMultiplier));
				mls.LogInfo(string.Format("Triple multiplier value from config: {0}", Plugin.CurrentUserConfig.configTripleMultiplier));
				mls.LogInfo(string.Format("Double multiplier value from config: {0}", Plugin.CurrentUserConfig.configDoubleMultiplier));
				mls.LogInfo(string.Format("Halve multiplier value from config: {0}", Plugin.CurrentUserConfig.configHalveMultiplier));
				mls.LogInfo(string.Format("Zero multiplier value from config: {0}", Plugin.CurrentUserConfig.configZeroMultiplier));
				mls.LogInfo(string.Format("Audio enabled from config: {0}", Plugin.CurrentUserConfig.configGamblingMusicEnabled));
				mls.LogInfo(string.Format("Audio volume from config: {0}", Plugin.CurrentUserConfig.configGamblingMusicVolume));
				mls.LogInfo(string.Format("Number of uses from config: {0}", Plugin.CurrentUserConfig.configNumberOfUses));
				mls.LogInfo(string.Format("Number of machines from config: {0}", Plugin.CurrentUserConfig.configNumberOfMachines));
				Plugin.mls.LogInfo("Successfully synced a client with host configuration");
			}
		}
	}
}
