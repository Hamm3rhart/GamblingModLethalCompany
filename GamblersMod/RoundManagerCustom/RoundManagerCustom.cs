using System.Collections.Generic;
using GamblersMod.config;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.RoundManagerCustomSpace
{
    internal class RoundManagerCustom : NetworkBehaviour
    {
        public RoundManager RoundManager;

        private List<Vector3> spawnPoints;

        void Awake()
        {
            RoundManager = GetComponent<RoundManager>();
            spawnPoints = new List<Vector3>();
            spawnPoints.Add(new Vector3(-27.808f, -2.6256f, -9.7409f));
            spawnPoints.Add(new Vector3(-27.808f, -2.6256f, -4.7409f));
            spawnPoints.Add(new Vector3(-27.808f, -2.6256f, 0.7409f));
            spawnPoints.Add(new Vector3(-27.808f, -2.6256f, 6.7409f));
        }

        [ServerRpc]
        public void DespawnGamblingMachineServerRpc()
        {
            GamblingMachineManager.Instance.DespawnAll();
        }

        [ServerRpc]
        public void SpawnGamblingMachineServerRpc()
        {
            Plugin.mls.LogInfo($"Attempting to spawn gambling machine at {RoundManager.currentLevel.name}");

            int capacity = spawnPoints.Count;
            string mode = (Plugin.CurrentUserConfig.configMachineSpawnMode ?? string.Empty).ToUpperInvariant();
            int spawnCount;

            if (mode == GambleConstants.MACHINE_SPAWN_MODE_MAX)
            {
                spawnCount = capacity;
            }
            else if (mode == GambleConstants.MACHINE_SPAWN_MODE_AUTO)
            {
                int playerCount = Mathf.Max(1, NetworkManager.Singleton?.ConnectedClients.Count ?? 1);
                spawnCount = Mathf.Min(capacity, playerCount);
            }
            else
            {
                spawnCount = capacity; // fallback
            }

            for (int i = 0; i < spawnCount; i++)
            {
                if (i >= spawnPoints.Count) return;
                GamblingMachineManager.Instance.Spawn(spawnPoints[i], Quaternion.Euler(0, 90, 0));
                Plugin.mls.LogInfo($"Spawned machine number: {i}");
            }
        }
    }
}
