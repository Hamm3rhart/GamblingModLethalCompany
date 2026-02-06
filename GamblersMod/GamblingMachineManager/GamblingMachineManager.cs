using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod
{
    public class GamblingMachineManager : MonoBehaviour
    {
        public List<GameObject> GamblingMachines;

        private GameObject musicEmitter;
        private AudioSource musicSource;

        public static GamblingMachineManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            Plugin.mls.LogMessage($"Gambling machine manager has awoken!");
            GamblingMachines = new List<GameObject>();
            DontDestroyOnLoad(gameObject);
        }

        public void Spawn(Vector3 spawnPoint, Quaternion quaternion)
        {
            Plugin.mls.LogMessage($"Spawning gambling machine... #{GamblingMachines.Count}");
            GameObject GamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, spawnPoint, quaternion);
            GamblingMachine.tag = "Untagged";
            GamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            GamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            GamblingMachine.GetComponent<NetworkObject>().Spawn();

            // Mute per-machine music sources; use central emitter instead
            var machineAudio = GamblingMachine.GetComponent<AudioSource>();
            if (machineAudio != null)
            {
                machineAudio.Pause();
            }

            GamblingMachines.Add(GamblingMachine);

            UpdateCentralMusicEmitter();
        }

        public void DespawnAll()
        {
            Plugin.mls.LogMessage($"Despwawning gambling machine...");
            foreach (GameObject GamblingMachine in GamblingMachines)
            {
                GamblingMachine.GetComponent<NetworkObject>().Despawn();
            }
            Reset();
        }

        public void Reset()
        {
            Plugin.mls.LogInfo("Resetting gambling machine manager state...");
            GamblingMachines.Clear();
            TeardownCentralMusicEmitter();
        }

        private void UpdateCentralMusicEmitter()
        {
            if (Plugin.CurrentUserConfig == null)
            {
                return;
            }

            if (!Plugin.CurrentUserConfig.configGamblingMusicEnabled || GamblingMachines.Count == 0)
            {
                TeardownCentralMusicEmitter();
                return;
            }

            if (musicEmitter == null)
            {
                musicEmitter = new GameObject("GamblingMusicEmitter");
                DontDestroyOnLoad(musicEmitter);
                musicSource = musicEmitter.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.spatialBlend = 1f; // 3D
                musicSource.rolloffMode = AudioRolloffMode.Linear;
                musicSource.minDistance = 8f;
                musicSource.maxDistance = 30f;
                musicSource.clip = Plugin.GamblingMachineMusicAudio;
            }

            musicSource.volume = Plugin.CurrentUserConfig.configGamblingMusicVolume;

            // Compute center as midpoint of bounding box (min/max) to cover grids evenly
            Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            int count = 0;
            foreach (var go in GamblingMachines)
            {
                if (go == null) continue;
                Vector3 p = go.transform.position;
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
                count++;
            }

            if (count == 0)
            {
                TeardownCentralMusicEmitter();
                return;
            }

            Vector3 center = (min + max) * 0.5f;
            musicEmitter.transform.position = center;

            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.Play();
            }
        }

        private void TeardownCentralMusicEmitter()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
            if (musicEmitter != null)
            {
                Destroy(musicEmitter);
                musicEmitter = null;
                musicSource = null;
            }
        }
    }
}
