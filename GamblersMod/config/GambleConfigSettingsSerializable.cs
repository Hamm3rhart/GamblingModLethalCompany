using System;
using BepInEx.Configuration;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.config
{
    [Serializable]
    public class GambleConfigSettingsSerializable
    {
        // General
        public int configMaxCooldown;
        public int configNumberOfUses;
        public int configMaxValueLimit;
        public string configMachineSpawnMode;
        public int configNumberOfRows;
        public int configMachinesPerRow;
        public float configRowSpacing;
        public float configColumnSpacing;
        public float configMachineRotation;
        public float configLayoutOffsetX;
        public float configLayoutOffsetY;
        public float configLayoutOffsetZ;

        // Gambling chances
        public int configJackpotChance;
        public int configTripleChance;
        public int configDoubleChance;
        public int configHalveChance;
        public int configZeroChance;
        public int configExplodeChance;

        // Gambling multipliers
        public float configJackpotMultiplier;
        public float configTripleMultiplier;
        public float configDoubleMultiplier;
        public float configHalveMultiplier;
        public float configZeroMultiplier;
        public float configExplodeMultiplier;

        // Audio
        public bool configGamblingMusicEnabled;
        public float configGamblingMusicVolume;

        public GambleConfigSettingsSerializable(ConfigFile configFile)
        {
            LoadLegacyLayoutDefaults(
                configFile,
                out var legacySpawnMode,
                out var legacyRows,
                out var legacyMachinesPerRow,
                out var legacyRowSpacing,
                out var legacyColumnSpacing,
                out var legacyRotation,
                out var legacyOffsetX,
                out var legacyOffsetY,
                out var legacyOffsetZ);

            // General
            configFile.Bind(GAMBLING_GENERAL_SECTION_KEY, CONFIG_MAXCOOLDOWN, 4, "Cooldown of the machine. Reducing this will cause the drumroll sound to not sync & may also cause latency issues");
            configFile.Bind(GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_USES, 9999, "Number of times a gambling machine can be used");
            configFile.Bind(GAMBLING_GENERAL_SECTION_KEY, CONFIG_MAX_VALUE_LIMIT, int.MaxValue, "Maximum scrap value after gambling. 0 or negative disables the cap");
            var spawnDefault = CoerceSpawnMode(legacySpawnMode);
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_MACHINE_SPAWN_MODE, spawnDefault,
                new ConfigDescription("Machine spawn mode: AUTO spawns up to the player count, MAX fills the grid capacity",
                new AcceptableValueList<string>(MACHINE_SPAWN_MODE_AUTO, MACHINE_SPAWN_MODE_MAX)));
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_NUMBER_OF_ROWS, legacyRows, "How many rows of gambling machines will be spawned (along X). Allows negatives in spacing to flip direction");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_MACHINES_PER_ROW, legacyMachinesPerRow, "How many gambling machines to place per row (along Z). Allows negatives in spacing to flip direction");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_ROW_SPACING, legacyRowSpacing, "Distance between rows of machines along X. Decimals allowed (e.g. 0.3). Zero falls back to 5");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_COLUMN_SPACING, legacyColumnSpacing, "Distance between machines in a row along Z. Decimals allowed (e.g. 0.3). Zero falls back to 5");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_MACHINE_ROTATION, legacyRotation, "Yaw rotation for each machine in degrees (0-359). Invalid values fall back to 90");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_LAYOUT_OFFSET_X, legacyOffsetX, "Offset the anchor position on the X axis (rows). Decimals allowed. 0 keeps the base point");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_LAYOUT_OFFSET_Y, legacyOffsetY, "Offset the anchor position on the Y axis (height). Decimals allowed. 0 keeps the base point");
            configFile.Bind(GAMBLING_LAYOUT_SECTION_KEY, CONFIG_LAYOUT_OFFSET_Z, legacyOffsetZ, "Offset the anchor position on the Z axis (columns). Decimals allowed. 0 keeps the base point");

            // Chance
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_JACKPOT_CHANCE_KEY, 3, "Chance to roll a jackpot. Ex. If set to 3, you have a 3% chance to get a jackpot. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_TRIPLE_CHANCE_KEY, 11, "Chance to roll a triple. Ex. If set to 11, you have a 11% chance to get a triple. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_DOUBLE_CHANCE_KEY, 27, "Chance to roll a double. Ex. If set to 27, you have a 27% chance to get a double. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_HALVE_CHANCE_KEY, 49, "Chance to roll a halve. Ex. If set to 49, you have a 49% chance to get a halve. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_ZERO_CHANCE_KEY, 9, "Chance to roll a zero. Ex. If set to 12, you have a 12% chance to get a zero. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_EXPLODE_CHANCE_KEY, 1, "Chance to explode. Make sure ALL your chance values add up to 100 or else the math won't make sense!");

            // Multipliers
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_JACKPOT_MULTIPLIER, 10f, "Jackpot multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_TRIPLE_MULTIPLIER, 3f, "Triple multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_DOUBLE_MULTIPLIER, 2f, "Double multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_HALVE_MULTIPLIER, 0.5f, "Halve multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_ZERO_MULTIPLIER, 0f, "Zero multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_EXPLODE_MULTIPLIER, 0f, "Explode multiplier applied before the explosion");

            // Audio
            configFile.Bind(GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_ENABLED, true, "Enable gambling machine music (CLIENT SIDE)");
            configFile.Bind(GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_VOLUME, 0.35f, "Gambling machine music volume (CLIENT SIDE)");

            configMaxCooldown = GetConfigFileKeyValue<int>(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_MAXCOOLDOWN);

            configJackpotChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_JACKPOT_CHANCE_KEY);
            configTripleChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_TRIPLE_CHANCE_KEY);
            configDoubleChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_DOUBLE_CHANCE_KEY);
            configHalveChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_HALVE_CHANCE_KEY);
            configZeroChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_ZERO_CHANCE_KEY);
            configExplodeChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_EXPLODE_CHANCE_KEY);

            configJackpotMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_JACKPOT_MULTIPLIER);
            configTripleMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_TRIPLE_MULTIPLIER);
            configDoubleMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_DOUBLE_MULTIPLIER);
            configHalveMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_HALVE_MULTIPLIER);
            configZeroMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_ZERO_MULTIPLIER);
            configExplodeMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_EXPLODE_MULTIPLIER);

            configGamblingMusicEnabled = GetConfigFileKeyValue<bool>(configFile, GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_ENABLED);
            configGamblingMusicVolume = GetConfigFileKeyValue<float>(configFile, GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_VOLUME);

            configNumberOfUses = GetConfigFileKeyValue<int>(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_USES);
            configMaxValueLimit = GetConfigFileKeyValue<int>(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_MAX_VALUE_LIMIT);
            configMachineSpawnMode = GetConfigFileKeyValue<string>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_MACHINE_SPAWN_MODE);
            configNumberOfRows = GetConfigFileKeyValue<int>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_NUMBER_OF_ROWS);
            configMachinesPerRow = GetConfigFileKeyValue<int>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_MACHINES_PER_ROW);
            configRowSpacing = GetConfigFileKeyValue<float>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_ROW_SPACING);
            configColumnSpacing = GetConfigFileKeyValue<float>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_COLUMN_SPACING);
            configMachineRotation = GetConfigFileKeyValue<float>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_MACHINE_ROTATION);
            configLayoutOffsetX = GetConfigFileKeyValue<float>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_LAYOUT_OFFSET_X);
            configLayoutOffsetY = GetConfigFileKeyValue<float>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_LAYOUT_OFFSET_Y);
            configLayoutOffsetZ = GetConfigFileKeyValue<float>(configFile, GAMBLING_LAYOUT_SECTION_KEY, CONFIG_LAYOUT_OFFSET_Z);

            LogInitializedConfigsValues();
        }

        private void LogInitializedConfigsValues()
        {
            var pluginLogger = Plugin.mls;
            pluginLogger.LogInfo($"Cooldown value from config: {configMaxCooldown}");

            pluginLogger.LogInfo($"Jackpot chance value from config: {configJackpotChance}");
            pluginLogger.LogInfo($"Triple chance value from config: {configTripleChance}");
            pluginLogger.LogInfo($"Double chance value from config: {configDoubleChance}");
            pluginLogger.LogInfo($"Halve chance value from config: {configHalveChance}");
            pluginLogger.LogInfo($"Zero chance value from config: {configZeroChance}");
            pluginLogger.LogInfo($"Explode chance value from config: {configExplodeChance}");

            pluginLogger.LogInfo($"Jackpot multiplier value from config: {configJackpotMultiplier}");
            pluginLogger.LogInfo($"Triple multiplier value from config: {configTripleMultiplier}");
            pluginLogger.LogInfo($"Double multiplier value from config: {configDoubleMultiplier}");
            pluginLogger.LogInfo($"Halve multiplier value from config: {configHalveMultiplier}");
            pluginLogger.LogInfo($"Zero multiplier value from config: {configZeroMultiplier}");
            pluginLogger.LogInfo($"Explode multiplier value from config: {configExplodeMultiplier}");

            pluginLogger.LogInfo($"Music enabled from config: {configGamblingMusicEnabled}");
            pluginLogger.LogInfo($"Music volume from config: {configGamblingMusicVolume}");

            pluginLogger.LogInfo($"Number of uses from config: {configNumberOfUses}");
            pluginLogger.LogInfo($"Max value limit from config: {configMaxValueLimit}");
            pluginLogger.LogInfo($"Machine spawn mode from config: {configMachineSpawnMode}");
            pluginLogger.LogInfo($"Number of rows from config: {configNumberOfRows}");
            pluginLogger.LogInfo($"Machines per row from config: {configMachinesPerRow}");
            pluginLogger.LogInfo($"Row spacing from config: {configRowSpacing}");
            pluginLogger.LogInfo($"Column spacing from config: {configColumnSpacing}");
            pluginLogger.LogInfo($"Machine rotation from config: {configMachineRotation}");
            pluginLogger.LogInfo($"Layout offset X from config: {configLayoutOffsetX}");
            pluginLogger.LogInfo($"Layout offset Y from config: {configLayoutOffsetY}");
            pluginLogger.LogInfo($"Layout offset Z from config: {configLayoutOffsetZ}");
        }

        private T GetConfigFileKeyValue<T>(ConfigFile configFile, string section, string key)
        {
            ConfigDefinition configDef = new ConfigDefinition(section, key);
            ConfigEntry<T> configEntry;
            Plugin.mls.LogInfo($"Getting configuration entry: Section: {section} Key: {key}");
            bool configExists = configFile.TryGetEntry(configDef, out configEntry);
            if (!configExists) Plugin.mls.LogError($"Failed to get configuration value. Section: {section} Key: {key}");
            return configEntry.Value;
        }

        private static void LoadLegacyLayoutDefaults(
            ConfigFile configFile,
            out string spawnMode,
            out int rows,
            out int machinesPerRow,
            out float rowSpacing,
            out float columnSpacing,
            out float rotation,
            out float offsetX,
            out float offsetY,
            out float offsetZ)
        {
            // Defaults if no legacy values are present
            spawnMode = MACHINE_SPAWN_MODE_AUTO;
            rows = 1;
            machinesPerRow = 12;
            rowSpacing = 5f;
            columnSpacing = 5f;
            rotation = 90f;
            offsetX = 0f;
            offsetY = 0f;
            offsetZ = 0f;

            // Pull legacy entries from the old General section if they exist, then remove them to avoid duplicate categories
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_MACHINE_SPAWN_MODE, ref spawnMode);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_ROWS, ref rows);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_MACHINES_PER_ROW, ref machinesPerRow);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_ROW_SPACING, ref rowSpacing);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_COLUMN_SPACING, ref columnSpacing);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_MACHINE_ROTATION, ref rotation);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_LAYOUT_OFFSET_X, ref offsetX);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_LAYOUT_OFFSET_Y, ref offsetY);
            TryReadLegacy(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_LAYOUT_OFFSET_Z, ref offsetZ);
        }

        private static string CoerceSpawnMode(string mode)
        {
            var upper = (mode ?? string.Empty).ToUpperInvariant();
            return upper == MACHINE_SPAWN_MODE_MAX ? MACHINE_SPAWN_MODE_MAX : MACHINE_SPAWN_MODE_AUTO;
        }

        private static void TryReadLegacy<T>(ConfigFile configFile, string section, string key, ref T target)
        {
            var def = new ConfigDefinition(section, key);
            if (configFile.TryGetEntry(def, out ConfigEntry<T> entry))
            {
                target = entry.Value;
                // remove the legacy entry to prevent duplicate categories
                configFile.Remove(def);
            }
        }
    }
}
