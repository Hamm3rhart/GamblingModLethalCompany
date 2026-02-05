using BepInEx.Logging;
using Unity.Collections;
using Unity.Netcode;

namespace GamblersMod.config
{
    public class GambleConfigNetworkHelper
    {
        public static void OnHostRecievesClientConfigRequest(ulong clientId, FastBufferReader _)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                return;
            }

            Plugin.mls.LogInfo("Host recieved client config request.");
            Plugin.mls.LogInfo("Serializing host config data...");

            byte[] serializedSettings = SerializerHelper.GetSerializedSettings(Plugin.CurrentUserConfig);

            Plugin.mls.LogInfo("Start writing host config data...");
            using (var writer = new FastBufferWriter(serializedSettings.Length + 4, Allocator.Temp, -1))
            {
                Plugin.mls.LogInfo("Writing host config data");
                int length = serializedSettings.Length;
                writer.WriteValueSafe(length);
                writer.WriteBytesSafe(serializedSettings);

                Plugin.mls.LogInfo($"Sending host config data to client with id of {clientId}...");
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage($"{Plugin.modGUID}_{GambleConstants.ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST}", clientId, writer, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        public static void StartClientRequestConfigFromHost()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                return;
            }

            Plugin.mls.LogInfo("Client is requesting configuration from host");
            using var writer = new FastBufferWriter(4, Allocator.Temp, -1);

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage($"{Plugin.modGUID}_{GambleConstants.ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST}", 0ul, writer, NetworkDelivery.ReliableSequenced);
        }

        public static void OnClientRecievesHostConfigRequest(ulong _, FastBufferReader reader)
        {
            Plugin.mls.LogInfo("Client recieved configuration message from host");

            if (!reader.TryBeginRead(4))
            {
                Plugin.mls.LogError("Could not sync client configuration with host. The stream sent by StartClientRequestConfigFromHost was invalid.");
                return;
            }

            reader.ReadValueSafe(out int configByteLength);

            if (!reader.TryBeginRead(configByteLength))
            {
                Plugin.mls.LogError("Could not sync client configuration with host. Host could not serialize the data.");
                return;
            }

            byte[] configByteData = new byte[configByteLength];
            reader.ReadBytesSafe(ref configByteData, configByteLength);

            Plugin.RecentHostConfig = SerializerHelper.GetDeserializedSettings<GambleConfigSettingsSerializable>(configByteData);
            Plugin.CurrentUserConfig = Plugin.RecentHostConfig;
            Plugin.CurrentUserConfig.configGamblingMusicEnabled = Plugin.UserConfigSnapshot.configGamblingMusicEnabled;
            Plugin.CurrentUserConfig.configGamblingMusicVolume = Plugin.UserConfigSnapshot.configGamblingMusicVolume;

            ManualLogSource mls = Plugin.mls;
            mls.LogInfo($"Cooldown value from config: {Plugin.CurrentUserConfig.configMaxCooldown}");
            mls.LogInfo($"Jackpot chance value from config: {Plugin.CurrentUserConfig.configJackpotChance}");
            mls.LogInfo($"Triple chance value from config: {Plugin.CurrentUserConfig.configTripleChance}");
            mls.LogInfo($"Double chance value from config: {Plugin.CurrentUserConfig.configDoubleChance}");
            mls.LogInfo($"Halve chance value from config: {Plugin.CurrentUserConfig.configHalveChance}");
            mls.LogInfo($"Zero chance value from config: {Plugin.CurrentUserConfig.configZeroChance}");
            mls.LogInfo($"Jackpot multiplier value from config: {Plugin.CurrentUserConfig.configJackpotMultiplier}");
            mls.LogInfo($"Triple multiplier value from config: {Plugin.CurrentUserConfig.configTripleMultiplier}");
            mls.LogInfo($"Double multiplier value from config: {Plugin.CurrentUserConfig.configDoubleMultiplier}");
            mls.LogInfo($"Halve multiplier value from config: {Plugin.CurrentUserConfig.configHalveMultiplier}");
            mls.LogInfo($"Zero multiplier value from config: {Plugin.CurrentUserConfig.configZeroMultiplier}");
            mls.LogInfo($"Audio enabled from config: {Plugin.CurrentUserConfig.configGamblingMusicEnabled}");
            mls.LogInfo($"Audio volume from config: {Plugin.CurrentUserConfig.configGamblingMusicVolume}");
            mls.LogInfo($"Number of uses from config: {Plugin.CurrentUserConfig.configNumberOfUses}");

            Plugin.mls.LogInfo("Successfully synced a client with host configuration");
        }
    }
}
