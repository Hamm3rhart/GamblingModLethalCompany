using System;

namespace GamblersMod.config
{
	// Token: 0x02000012 RID: 18
	public class GambleConstants
	{
		// Token: 0x04000045 RID: 69
		public static readonly string GAMBLING_GENERAL_SECTION_KEY = "General Machine Settings";

		// Token: 0x04000046 RID: 70
		public static readonly string GAMBLING_CHANCE_SECTION_KEY = "Gambling Chances";

		// Token: 0x04000047 RID: 71
		public static readonly string GAMBLING_MULTIPLIERS_SECTION_KEY = "Gambling Multipliers";

		// Token: 0x04000048 RID: 72
		public static readonly string GAMBLING_AUDIO_SECTION_KEY = "Audio";

		// Token: 0x04000049 RID: 73
		public static readonly string CONFIG_MAXCOOLDOWN = "gamblingMachineMaxCooldown";

		// Token: 0x0400004A RID: 74
		public static readonly string CONFIG_NUMBER_OF_USES = "Number Of Uses";

		// Token: 0x0400004B RID: 75
		public static readonly string CONFIG_NUMBER_OF_MACHINES = "Number Of Machines";

		// Token: 0x0400004C RID: 76
		public static readonly string CONFIG_JACKPOT_CHANCE_KEY = "JackpotChance";

		// Token: 0x0400004D RID: 77
		public static readonly string CONFIG_TRIPLE_CHANCE_KEY = "TripleChance";

		// Token: 0x0400004E RID: 78
		public static readonly string CONFIG_DOUBLE_CHANCE_KEY = "DoubleChance";

		// Token: 0x0400004F RID: 79
		public static readonly string CONFIG_HALVE_CHANCE_KEY = "HalveChance";

		// Token: 0x04000050 RID: 80
		public static readonly string CONFIG_ZERO_CHANCE_KEY = "ZeroChance";

		// Token: 0x04000051 RID: 81
		public static readonly string CONFIG_JACKPOT_MULTIPLIER = "JackpotMultiplier";

		// Token: 0x04000052 RID: 82
		public static readonly string CONFIG_TRIPLE_MULTIPLIER = "TripleMultiplier";

		// Token: 0x04000053 RID: 83
		public static readonly string CONFIG_DOUBLE_MULTIPLIER = "DoubleMultiplier";

		// Token: 0x04000054 RID: 84
		public static readonly string CONFIG_HALVE_MULTIPLIER = "HalveMultiplier";

		// Token: 0x04000055 RID: 85
		public static readonly string CONFIG_ZERO_MULTIPLIER = "ZeroMultiplier";

		// Token: 0x04000056 RID: 86
		public static readonly string CONFIG_GAMBLING_MUSIC_ENABLED = "GambleMachineMusicEnabled";

		// Token: 0x04000057 RID: 87
		public static readonly string CONFIG_GAMBLING_MUSIC_VOLUME = "GambleMachineMusicVolume";

		// Token: 0x04000058 RID: 88
		public static readonly string ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST = "OnHostRecievesClientConfigRequest";

		// Token: 0x04000059 RID: 89
		public static readonly string ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST = "OnClientRecievesHostConfigRequest";

		// Token: 0x02000013 RID: 19
		public struct GamblingOutcome
		{
			// Token: 0x0400005A RID: 90
			public static string JACKPOT = "JACKPOT";

			// Token: 0x0400005B RID: 91
			public static string TRIPLE = "TRIPLE";

			// Token: 0x0400005C RID: 92
			public static string DOUBLE = "DOUBLE";

			// Token: 0x0400005D RID: 93
			public static string HALVE = "HALVE";

			// Token: 0x0400005E RID: 94
			public static string REMOVE = "REMOVE";

			// Token: 0x0400005F RID: 95
			public static string DEFAULT = "DEFAULT";
		}
	}
}
