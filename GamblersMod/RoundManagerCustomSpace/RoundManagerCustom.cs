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
            spawnPoints = new List<Vector3>();

            for (int i = 0; i < 12; i++)
            {
                spawnPoints.Add(new Vector3(-27.808f, -2.6256f, -14.7409f + (i * 5)));
            }
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

            for (int i = 0; i < Plugin.CurrentUserConfig.configNumberOfMachines; i++)
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
