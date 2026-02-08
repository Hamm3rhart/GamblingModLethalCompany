using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod
{
	// Token: 0x02000002 RID: 2
	public class GamblingMachineManager : MonoBehaviour
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public static GamblingMachineManager Instance { get; private set; }

		// Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
		private void Awake()
		{
			bool flag = GamblingMachineManager.Instance == null;
			if (flag)
			{
				GamblingMachineManager.Instance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
			Plugin.mls.LogMessage("Gambling machine manager has awoken!");
			this.GamblingMachines = new List<GameObject>();
			Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020BC File Offset: 0x000002BC
		public void Spawn(Vector3 spawnPoint, Quaternion quaternion)
		{
			Plugin.mls.LogMessage(string.Format("Spawning gambling machine... #{0}", this.GamblingMachines.Count));
			GameObject gameObject = Object.Instantiate<GameObject>(Plugin.GamblingMachine, spawnPoint, quaternion);
			gameObject.tag = "Untagged";
			gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			gameObject.layer = LayerMask.NameToLayer("InteractableObject");
			gameObject.GetComponent<NetworkObject>().Spawn(false);
			bool flag = this.GamblingMachines.Count >= 1;
			if (flag)
			{
				gameObject.GetComponent<AudioSource>().Pause();
			}
			this.GamblingMachines.Add(gameObject);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002178 File Offset: 0x00000378
		public void DespawnAll()
		{
			Plugin.mls.LogMessage("Despwawning gambling machine...");
			foreach (GameObject gameObject in this.GamblingMachines)
			{
				gameObject.GetComponent<NetworkObject>().Despawn(true);
			}
			this.Reset();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
		public void Reset()
		{
			Plugin.mls.LogInfo("Resetting gambling machine manager state...");
			this.GamblingMachines.Clear();
		}

		// Token: 0x04000001 RID: 1
		public List<GameObject> GamblingMachines;
	}
}
