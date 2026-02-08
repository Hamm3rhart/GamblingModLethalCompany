using System;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace GamblersMod.config
{
	// Token: 0x02000014 RID: 20
	[Serializable]
	public class GambleConfigSettingsSerializable
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00004B50 File Offset: 0x00002D50
		public GambleConfigSettingsSerializable(ConfigFile configFile)
		{
			configFile.Bind<int>(GambleConstants.GAMBLING_GENERAL_SECTION_KEY, GambleConstants.CONFIG_MAXCOOLDOWN, 4, "Cooldown of the machine. Reducing this will cause the drumroll sound to not sync & may also cause latency issues");
			configFile.Bind<int>(GambleConstants.GAMBLING_GENERAL_SECTION_KEY, GambleConstants.CONFIG_NUMBER_OF_USES, 9999, "Number of times a gambling machine can be used");
			configFile.Bind<int>(GambleConstants.GAMBLING_GENERAL_SECTION_KEY, GambleConstants.CONFIG_NUMBER_OF_MACHINES, 3, "How many gambling machines will be spawned (max 4)");
			configFile.Bind<int>(GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_JACKPOT_CHANCE_KEY, 3, "Chance to roll a jackpot. Ex. If set to 3, you have a 3% chance to get a jackpot. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
			configFile.Bind<int>(GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_TRIPLE_CHANCE_KEY, 11, "Chance to roll a triple. Ex. If set to 11, you have a 11% chance to get a triple. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
			configFile.Bind<int>(GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_DOUBLE_CHANCE_KEY, 27, "Chance to roll a double. Ex. If set to 27, you have a 27% chance to get a double. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
			configFile.Bind<int>(GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_HALVE_CHANCE_KEY, 50, "Chance to roll a halve. Ex. If set to 47, you have a 47% chance to get a halve. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
			configFile.Bind<int>(GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_ZERO_CHANCE_KEY, 9, "Chance to roll a zero. Ex. If set to 12, you have a 12% chance to get a zero. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
			configFile.Bind<float>(GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_JACKPOT_MULTIPLIER, 10f, "Jackpot multiplier");
			configFile.Bind<float>(GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_TRIPLE_MULTIPLIER, 3f, "Triple multiplier");
			configFile.Bind<float>(GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_DOUBLE_MULTIPLIER, 2f, "Double multiplier");
			configFile.Bind<float>(GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_HALVE_MULTIPLIER, 0.5f, "Halve multiplier");
			configFile.Bind<float>(GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_ZERO_MULTIPLIER, 0f, "Zero multiplier");
			configFile.Bind<bool>(GambleConstants.GAMBLING_AUDIO_SECTION_KEY, GambleConstants.CONFIG_GAMBLING_MUSIC_ENABLED, true, "Enable gambling machine music (CLIENT SIDE)");
			configFile.Bind<float>(GambleConstants.GAMBLING_AUDIO_SECTION_KEY, GambleConstants.CONFIG_GAMBLING_MUSIC_VOLUME, 0.35f, "Gambling machine music volume (CLIENT SIDE)");
			this.configMaxCooldown = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_GENERAL_SECTION_KEY, GambleConstants.CONFIG_MAXCOOLDOWN);
			this.configJackpotChance = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_JACKPOT_CHANCE_KEY);
			this.configTripleChance = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_TRIPLE_CHANCE_KEY);
			this.configDoubleChance = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_DOUBLE_CHANCE_KEY);
			this.configHalveChance = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_HALVE_CHANCE_KEY);
			this.configZeroChance = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_CHANCE_SECTION_KEY, GambleConstants.CONFIG_ZERO_CHANCE_KEY);
			this.configJackpotMultiplier = this.GetConfigFileKeyValue<float>(configFile, GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_JACKPOT_MULTIPLIER);
			this.configTripleMultiplier = this.GetConfigFileKeyValue<float>(configFile, GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_TRIPLE_MULTIPLIER);
			this.configDoubleMultiplier = this.GetConfigFileKeyValue<float>(configFile, GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_DOUBLE_MULTIPLIER);
			this.configHalveMultiplier = this.GetConfigFileKeyValue<float>(configFile, GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_HALVE_MULTIPLIER);
			this.configZeroMultiplier = this.GetConfigFileKeyValue<float>(configFile, GambleConstants.GAMBLING_MULTIPLIERS_SECTION_KEY, GambleConstants.CONFIG_ZERO_MULTIPLIER);
			this.configGamblingMusicEnabled = this.GetConfigFileKeyValue<bool>(configFile, GambleConstants.GAMBLING_AUDIO_SECTION_KEY, GambleConstants.CONFIG_GAMBLING_MUSIC_ENABLED);
			this.configGamblingMusicVolume = this.GetConfigFileKeyValue<float>(configFile, GambleConstants.GAMBLING_AUDIO_SECTION_KEY, GambleConstants.CONFIG_GAMBLING_MUSIC_VOLUME);
			this.configNumberOfUses = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_GENERAL_SECTION_KEY, GambleConstants.CONFIG_NUMBER_OF_USES);
			this.configNumberOfMachines = this.GetConfigFileKeyValue<int>(configFile, GambleConstants.GAMBLING_GENERAL_SECTION_KEY, GambleConstants.CONFIG_NUMBER_OF_MACHINES);
			this.LogInitializedConfigsValues();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004E40 File Offset: 0x00003040
		private void LogInitializedConfigsValues()
		{
			ManualLogSource mls = Plugin.mls;
			mls.LogInfo(string.Format("Cooldown value from config: {0}", this.configMaxCooldown));
			mls.LogInfo(string.Format("Jackpot chance value from config: {0}", this.configJackpotChance));
			mls.LogInfo(string.Format("Triple chance value from config: {0}", this.configTripleChance));
			mls.LogInfo(string.Format("Double chance value from config: {0}", this.configDoubleChance));
			mls.LogInfo(string.Format("Halve chance value from config: {0}", this.configHalveChance));
			mls.LogInfo(string.Format("Zero chance value from config: {0}", this.configZeroChance));
			mls.LogInfo(string.Format("Jackpot multiplier value from config: {0}", this.configJackpotMultiplier));
			mls.LogInfo(string.Format("Triple multiplier value from config: {0}", this.configTripleMultiplier));
			mls.LogInfo(string.Format("Double multiplier value from config: {0}", this.configDoubleMultiplier));
			mls.LogInfo(string.Format("Halve multiplier value from config: {0}", this.configHalveMultiplier));
			mls.LogInfo(string.Format("Zero multiplier value from config: {0}", this.configZeroMultiplier));
			mls.LogInfo(string.Format("Music enabled from config: {0}", this.configGamblingMusicEnabled));
			mls.LogInfo(string.Format("Music volume from config: {0}", this.configGamblingMusicVolume));
			mls.LogInfo(string.Format("Number of uses from config: {0}", this.configNumberOfUses));
			mls.LogInfo(string.Format("Number of machines from config: {0}", this.configNumberOfMachines));
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004FF8 File Offset: 0x000031F8
		private T GetConfigFileKeyValue<T>(ConfigFile configFile, string section, string key)
		{
			ConfigDefinition configDefinition = new ConfigDefinition(section, key);
			Plugin.mls.LogInfo("Getting configuration entry: Section: " + section + " Key: " + key);
			ConfigEntry<T> configEntry;
			bool flag = configFile.TryGetEntry<T>(configDefinition, ref configEntry);
			bool flag2 = !flag;
			if (flag2)
			{
				Plugin.mls.LogError("Failed to get configuration value. Section: " + section + " Key: " + key);
			}
			return configEntry.Value;
		}

		// Token: 0x04000060 RID: 96
		public int configMaxCooldown;

		// Token: 0x04000061 RID: 97
		public int configNumberOfUses;

		// Token: 0x04000062 RID: 98
		public int configNumberOfMachines;

		// Token: 0x04000063 RID: 99
		public int configJackpotChance;

		// Token: 0x04000064 RID: 100
		public int configTripleChance;

		// Token: 0x04000065 RID: 101
		public int configDoubleChance;

		// Token: 0x04000066 RID: 102
		public int configHalveChance;

		// Token: 0x04000067 RID: 103
		public int configZeroChance;

		// Token: 0x04000068 RID: 104
		public float configJackpotMultiplier;

		// Token: 0x04000069 RID: 105
		public float configTripleMultiplier;

		// Token: 0x0400006A RID: 106
		public float configDoubleMultiplier;

		// Token: 0x0400006B RID: 107
		public float configHalveMultiplier;

		// Token: 0x0400006C RID: 108
		public float configZeroMultiplier;

		// Token: 0x0400006D RID: 109
		public bool configGamblingMusicEnabled;

		// Token: 0x0400006E RID: 110
		public float configGamblingMusicVolume;
	}
}
