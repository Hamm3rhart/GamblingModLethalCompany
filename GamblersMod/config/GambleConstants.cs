using System;

namespace GamblersMod.config
{
    // Token: 0x02000012 RID: 18
    public class GambleConstants
    {
        public static readonly string GAMBLING_GENERAL_SECTION_KEY = "General Machine Settings";
        public static readonly string GAMBLING_CHANCE_SECTION_KEY = "Gambling Chances";
        public static readonly string GAMBLING_MULTIPLIERS_SECTION_KEY = "Gambling Multipliers";
        public static readonly string GAMBLING_AUDIO_SECTION_KEY = "Audio";
        public static readonly string GAMBLING_LAYOUT_SECTION_KEY = "Machine Layout";

        public static readonly string CONFIG_MAXCOOLDOWN = "gamblingMachineMaxCooldown";
        public static readonly string CONFIG_NUMBER_OF_USES = "Number Of Uses";
        public static readonly string CONFIG_MACHINE_SPAWN_MODE = "Machine Spawn Mode";
        public static readonly string CONFIG_NUMBER_OF_ROWS = "Number Of Rows";
        public static readonly string CONFIG_MACHINES_PER_ROW = "Machines Per Row";
        public static readonly string CONFIG_ROW_SPACING = "Row Spacing";
        public static readonly string CONFIG_COLUMN_SPACING = "Machine Spacing";

        public const string MACHINE_SPAWN_MODE_AUTO = "AUTO";
        public const string MACHINE_SPAWN_MODE_MAX = "MAX";

        public static readonly string CONFIG_JACKPOT_CHANCE_KEY = "JackpotChance";
        public static readonly string CONFIG_TRIPLE_CHANCE_KEY = "TripleChance";
        public static readonly string CONFIG_DOUBLE_CHANCE_KEY = "DoubleChance";
        public static readonly string CONFIG_HALVE_CHANCE_KEY = "HalveChance";
        public static readonly string CONFIG_ZERO_CHANCE_KEY = "ZeroChance";

        public static readonly string CONFIG_JACKPOT_MULTIPLIER = "JackpotMultiplier";
        public static readonly string CONFIG_TRIPLE_MULTIPLIER = "TripleMultiplier";
        public static readonly string CONFIG_DOUBLE_MULTIPLIER = "DoubleMultiplier";
        public static readonly string CONFIG_HALVE_MULTIPLIER = "HalveMultiplier";
        public static readonly string CONFIG_ZERO_MULTIPLIER = "ZeroMultiplier";

        public static readonly string CONFIG_GAMBLING_MUSIC_ENABLED = "GambleMachineMusicEnabled";
        public static readonly string CONFIG_GAMBLING_MUSIC_VOLUME = "GambleMachineMusicVolume";

        public static readonly string ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST = "OnHostRecievesClientConfigRequest";
        public static readonly string ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST = "OnClientRecievesHostConfigRequest";

        public struct GamblingOutcome
        {
            public static string JACKPOT = "JACKPOT";
            public static string TRIPLE = "TRIPLE";
            public static string DOUBLE = "DOUBLE";
            public static string HALVE = "HALVE";
            public static string REMOVE = "REMOVE";
            public static string DEFAULT = "DEFAULT";
        }
    }
}
