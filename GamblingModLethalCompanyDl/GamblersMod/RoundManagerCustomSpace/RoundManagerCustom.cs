using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.RoundManagerCustomSpace
{
	// Token: 0x02000005 RID: 5
	internal class RoundManagerCustom : NetworkBehaviour
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002730 File Offset: 0x00000930
		private void Awake()
		{
			this.RoundManager = base.GetComponent<RoundManager>();
			this.spawnPoints = new List<Vector3>();
			for (int i = 0; i < 12; i++)
			{
				this.spawnPoints.Add(new Vector3(-27.808f, -2.6256f, -14.7409f + (float)(i * 5)));
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002790 File Offset: 0x00000990
		[ServerRpc]
		public void DespawnGamblingMachineServerRpc()
		{
			NetworkManager networkManager = base.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (this.__rpc_exec_stage != 1 && (networkManager.IsClient || networkManager.IsHost))
			{
				if (base.OwnerClientId != networkManager.LocalClientId)
				{
					if (networkManager.LogLevel <= 1)
					{
						Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}
					return;
				}
				ServerRpcParams serverRpcParams;
				FastBufferWriter fastBufferWriter = base.__beginSendServerRpc(3258984052U, serverRpcParams, 0);
				base.__endSendServerRpc(ref fastBufferWriter, 3258984052U, serverRpcParams, 0);
			}
			if (this.__rpc_exec_stage != 1 || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			this.__rpc_exec_stage = 0;
			GamblingMachineManager.Instance.DespawnAll();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000028B8 File Offset: 0x00000AB8
		[ServerRpc]
		public void SpawnGamblingMachineServerRpc()
		{
			NetworkManager networkManager = base.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (this.__rpc_exec_stage != 1 && (networkManager.IsClient || networkManager.IsHost))
			{
				if (base.OwnerClientId != networkManager.LocalClientId)
				{
					if (networkManager.LogLevel <= 1)
					{
						Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}
					return;
				}
				ServerRpcParams serverRpcParams;
				FastBufferWriter fastBufferWriter = base.__beginSendServerRpc(2764001088U, serverRpcParams, 0);
				base.__endSendServerRpc(ref fastBufferWriter, 2764001088U, serverRpcParams, 0);
			}
			if (this.__rpc_exec_stage != 1 || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			this.__rpc_exec_stage = 0;
			Plugin.mls.LogInfo("Attempting to spawn gambling machine at " + this.RoundManager.currentLevel.name);
			for (int i = 0; i < Plugin.CurrentUserConfig.configNumberOfMachines; i++)
			{
				bool flag = i >= this.spawnPoints.Count;
				if (flag)
				{
					break;
				}
				GamblingMachineManager.Instance.Spawn(this.spawnPoints[i], Quaternion.Euler(0f, 90f, 0f));
				Plugin.mls.LogInfo(string.Format("Spawned machine number: {0}", i));
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002A70 File Offset: 0x00000C70
		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002A88 File Offset: 0x00000C88
		protected override void __initializeRpcs()
		{
			base.__registerRpc(3258984052U, new NetworkBehaviour.RpcReceiveHandler(RoundManagerCustom.__rpc_handler_3258984052), "DespawnGamblingMachineServerRpc");
			base.__registerRpc(2764001088U, new NetworkBehaviour.RpcReceiveHandler(RoundManagerCustom.__rpc_handler_2764001088), "SpawnGamblingMachineServerRpc");
			base.__initializeRpcs();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002AD8 File Offset: 0x00000CD8
		private static void __rpc_handler_3258984052(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
			{
				if (networkManager.LogLevel <= 1)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			target.__rpc_exec_stage = 1;
			((RoundManagerCustom)target).DespawnGamblingMachineServerRpc();
			target.__rpc_exec_stage = 0;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002B78 File Offset: 0x00000D78
		private static void __rpc_handler_2764001088(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
			{
				if (networkManager.LogLevel <= 1)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			target.__rpc_exec_stage = 1;
			((RoundManagerCustom)target).SpawnGamblingMachineServerRpc();
			target.__rpc_exec_stage = 0;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002C16 File Offset: 0x00000E16
		protected internal override string __getTypeName()
		{
			return "RoundManagerCustom";
		}

		// Token: 0x04000018 RID: 24
		public RoundManager RoundManager;

		// Token: 0x04000019 RID: 25
		private List<Vector3> spawnPoints;
	}
}
