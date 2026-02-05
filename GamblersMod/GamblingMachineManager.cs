using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod
{
    public class GamblingMachineManager : MonoBehaviour
    {
        public static GamblingMachineManager Instance { get; private set; }

        public List<GameObject> GamblingMachines;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            Plugin.mls.LogMessage("Gambling machine manager has awoken!");
            GamblingMachines = new List<GameObject>();
            DontDestroyOnLoad(gameObject);
        }

        public void Spawn(Vector3 spawnPoint, Quaternion quaternion)
        {
            Plugin.mls.LogMessage($"Spawning gambling machine... #{GamblingMachines.Count}");
            GameObject gamblingMachine = Instantiate(Plugin.GamblingMachine, spawnPoint, quaternion);
            gamblingMachine.tag = "Untagged";
            gamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            gamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            gamblingMachine.GetComponent<NetworkObject>().Spawn(false);

            GamblingMachines.Add(gamblingMachine);
        }

        public void DespawnAll()
        {
            Plugin.mls.LogMessage("Despwawning gambling machine...");
            foreach (GameObject gamblingMachine in GamblingMachines)
            {
                gamblingMachine.GetComponent<NetworkObject>().Despawn(true);
            }

            Reset();
        }

        public void Reset()
        {
            Plugin.mls.LogInfo("Resetting gambling machine manager state...");
            GamblingMachines.Clear();
        }
    }
}
