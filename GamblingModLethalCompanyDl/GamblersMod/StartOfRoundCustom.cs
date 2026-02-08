using System;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod
{
	// Token: 0x02000004 RID: 4
	public class StartOfRoundCustom : NetworkBehaviour
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002518 File Offset: 0x00000718
		private void Awake()
		{
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000251C File Offset: 0x0000071C
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
				FastBufferWriter fastBufferWriter = base.__beginSendServerRpc(1680317104U, serverRpcParams, 0);
				base.__endSendServerRpc(ref fastBufferWriter, 1680317104U, serverRpcParams, 0);
			}
			if (this.__rpc_exec_stage != 1 || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			this.__rpc_exec_stage = 0;
			GamblingMachineManager.Instance.DespawnAll();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000264C File Offset: 0x0000084C
		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002662 File Offset: 0x00000862
		protected override void __initializeRpcs()
		{
			base.__registerRpc(1680317104U, new NetworkBehaviour.RpcReceiveHandler(StartOfRoundCustom.__rpc_handler_1680317104), "DespawnGamblingMachineServerRpc");
			base.__initializeRpcs();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002688 File Offset: 0x00000888
		private static void __rpc_handler_1680317104(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
			((StartOfRoundCustom)target).DespawnGamblingMachineServerRpc();
			target.__rpc_exec_stage = 0;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002726 File Offset: 0x00000926
		protected internal override string __getTypeName()
		{
			return "StartOfRoundCustom";
		}
	}
}
