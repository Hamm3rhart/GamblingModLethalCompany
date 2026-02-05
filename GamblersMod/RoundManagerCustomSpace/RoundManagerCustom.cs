using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.RoundManagerCustomSpace
{
    internal class RoundManagerCustom : NetworkBehaviour
    {
        public RoundManager RoundManager;
        private List<Vector3> spawnPoints;

        private void Awake()
        {
            RoundManager = GetComponent<RoundManager>();
            BuildSpawnPointsFromConfig();
        }

        private void BuildSpawnPointsFromConfig()
        {
            spawnPoints = new List<Vector3>();

            var cfg = Plugin.CurrentUserConfig;

            int rows = Mathf.Max(1, cfg.configNumberOfRows);
            int perRow = Mathf.Max(1, cfg.configMachinesPerRow);

            // Enforce sensible spacing even if old configs have 0
            float rowSpacing = cfg.configRowSpacing <= 0f ? 5f : cfg.configRowSpacing;          // forward/back between rows
            float columnSpacing = cfg.configColumnSpacing <= 0f ? 5f : cfg.configColumnSpacing;  // left/right within a row

            // Anchor at the original spawn point used previously
            Vector3 basePoint = new Vector3(-27.808f, -2.6256f, -14.7409f);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < perRow; col++)
                {
                    Vector3 offset = new Vector3(col * columnSpacing, 0f, row * rowSpacing);
                    spawnPoints.Add(basePoint + offset);
                }
            }
        }

        private int CalculateSpawnCount()
        {
            var cfg = Plugin.CurrentUserConfig;
            int capacity = spawnPoints.Count;
            string mode = (cfg.configMachineSpawnMode ?? string.Empty).ToUpperInvariant();

            if (mode == GamblersMod.config.GambleConstants.MACHINE_SPAWN_MODE_MAX)
            {
                return capacity;
            }

            if (mode == GamblersMod.config.GambleConstants.MACHINE_SPAWN_MODE_AUTO)
            {
                int playerCount = 1;
                if (NetworkManager.Singleton != null)
                {
                    playerCount = Mathf.Max(1, NetworkManager.Singleton.ConnectedClients.Count);
                }
                return Mathf.Min(capacity, playerCount);
            }

            // Fallback to MAX
            return capacity;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DespawnGamblingMachineServerRpc()
        {
            if (!IsServer)
            {
                return;
            }

            GamblingMachineManager.Instance.DespawnAll();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnGamblingMachineServerRpc()
        {
            if (!IsServer)
            {
                return;
            }

            Plugin.mls.LogInfo($"Attempting to spawn gambling machine at {RoundManager.currentLevel.name}");

            // Rebuild in case config was changed after Awake (e.g. synced from host)
            BuildSpawnPointsFromConfig();

            int spawnCount = CalculateSpawnCount();

            Plugin.mls.LogInfo($"Spawning up to {spawnCount} gambling machines (capacity {spawnPoints.Count})");

            for (int i = 0; i < spawnCount; i++)
            {
                if (i >= spawnPoints.Count)
                {
                    break;
                }

                GamblingMachineManager.Instance.Spawn(spawnPoints[i], Quaternion.Euler(0f, 90f, 0f));
                Plugin.mls.LogInfo($"Spawned machine number: {i}");
            }
        }
    }
}
