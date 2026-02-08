using System;
using System.Collections;
using GamblersMod.config;
using GamblersMod.Player;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.Patches
{
	// Token: 0x0200000B RID: 11
	internal class GamblingMachine : NetworkBehaviour
	{
		// Token: 0x06000034 RID: 52 RVA: 0x000033B0 File Offset: 0x000015B0
		private void Awake()
		{
			Plugin.mls.LogInfo("GamblingMachine has Awoken");
			this.gamblingMachineMaxCooldown = Plugin.CurrentUserConfig.configMaxCooldown;
			this.numberOfUses = Plugin.CurrentUserConfig.configNumberOfUses;
			this.jackpotMultiplier = Plugin.CurrentUserConfig.configJackpotMultiplier;
			this.tripleMultiplier = Plugin.CurrentUserConfig.configTripleMultiplier;
			this.doubleMultiplier = Plugin.CurrentUserConfig.configDoubleMultiplier;
			this.halvedMultiplier = Plugin.CurrentUserConfig.configHalveMultiplier;
			this.zeroMultiplier = Plugin.CurrentUserConfig.configZeroMultiplier;
			this.jackpotPercentage = Plugin.CurrentUserConfig.configJackpotChance;
			this.triplePercentage = Plugin.CurrentUserConfig.configTripleChance;
			this.doublePercentage = Plugin.CurrentUserConfig.configDoubleChance;
			this.halvedPercentage = Plugin.CurrentUserConfig.configHalveChance;
			this.removedPercentage = Plugin.CurrentUserConfig.configZeroChance;
			this.isMusicEnabled = Plugin.CurrentUserConfig.configGamblingMusicEnabled;
			this.musicVolume = Plugin.CurrentUserConfig.configGamblingMusicVolume;
			Plugin.mls.LogInfo(string.Format("GamblingMachine: gamblingMachineMaxCooldown loaded from config: {0}", this.gamblingMachineMaxCooldown));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: jackpotMultiplier loaded from config: {0}", this.jackpotMultiplier));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: tripleMultiplier loaded from config: {0}", this.tripleMultiplier));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: doubleMultiplier loaded from config: {0}", this.doubleMultiplier));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: halvedMultiplier loaded from config: {0}", this.halvedMultiplier));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: zeroMultiplier loaded from config: {0}", this.zeroMultiplier));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: jackpotPercentage loaded from config: {0}", this.jackpotPercentage));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: triplePercentage loaded from config: {0}", this.triplePercentage));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: doublePercentage loaded from config: {0}", this.doublePercentage));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: halvedPercentage loaded from config: {0}", this.halvedPercentage));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: removedPercentage loaded from config: {0}", this.removedPercentage));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: gamblingMusicEnabled loaded from config: {0}", this.isMusicEnabled));
			Plugin.mls.LogInfo(string.Format("GamblingMachine: gamblingMusicVolume loaded from config: {0}", this.musicVolume));
			this.InitAudioSource();
			this.rollMinValue = 1;
			this.rollMaxValue = this.jackpotPercentage + this.triplePercentage + this.doublePercentage + this.halvedPercentage + this.removedPercentage;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003684 File Offset: 0x00001884
		private void Start()
		{
			Plugin.mls.LogInfo("GamblingMachine has Started");
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003698 File Offset: 0x00001898
		public void GenerateGamblingOutcomeFromCurrentRoll()
		{
			bool flag = this.currentRoll >= this.rollMinValue && this.currentRoll <= this.jackpotPercentage;
			int num = this.jackpotPercentage;
			int num2 = this.jackpotPercentage + this.triplePercentage;
			bool flag2 = this.currentRoll > num && this.currentRoll <= num2;
			int num3 = num2;
			int num4 = num2 + this.doublePercentage;
			bool flag3 = this.currentRoll > num3 && this.currentRoll <= num4;
			int num5 = num4;
			int num6 = num4 + this.halvedPercentage;
			bool flag4 = this.currentRoll > num5 && this.currentRoll <= num6;
			bool flag5 = flag;
			if (flag5)
			{
				Plugin.mls.LogMessage("Rolled Jackpot");
				this.currentGamblingOutcomeMultiplier = this.jackpotMultiplier;
				this.currentGamblingOutcome = GambleConstants.GamblingOutcome.JACKPOT;
			}
			else
			{
				bool flag6 = flag2;
				if (flag6)
				{
					Plugin.mls.LogMessage("Rolled Triple");
					this.currentGamblingOutcomeMultiplier = this.tripleMultiplier;
					this.currentGamblingOutcome = GambleConstants.GamblingOutcome.TRIPLE;
				}
				else
				{
					bool flag7 = flag3;
					if (flag7)
					{
						Plugin.mls.LogMessage("Rolled Double");
						this.currentGamblingOutcomeMultiplier = this.doubleMultiplier;
						this.currentGamblingOutcome = GambleConstants.GamblingOutcome.DOUBLE;
					}
					else
					{
						bool flag8 = flag4;
						if (flag8)
						{
							Plugin.mls.LogMessage("Rolled Halved");
							this.currentGamblingOutcomeMultiplier = this.halvedMultiplier;
							this.currentGamblingOutcome = GambleConstants.GamblingOutcome.HALVE;
						}
						else
						{
							Plugin.mls.LogMessage("Rolled Remove");
							this.currentGamblingOutcomeMultiplier = this.zeroMultiplier;
							this.currentGamblingOutcome = GambleConstants.GamblingOutcome.REMOVE;
						}
					}
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003844 File Offset: 0x00001A44
		public void PlayGambleResultAudio(string outcome)
		{
			bool flag = outcome == GambleConstants.GamblingOutcome.JACKPOT;
			if (flag)
			{
				AudioSource.PlayClipAtPoint(Plugin.GamblingJackpotScrapAudio, base.transform.position, 0.6f);
			}
			else
			{
				bool flag2 = outcome == GambleConstants.GamblingOutcome.TRIPLE;
				if (flag2)
				{
					AudioSource.PlayClipAtPoint(Plugin.GamblingTripleScrapAudio, base.transform.position, 0.6f);
				}
				else
				{
					bool flag3 = outcome == GambleConstants.GamblingOutcome.DOUBLE;
					if (flag3)
					{
						AudioSource.PlayClipAtPoint(Plugin.GamblingDoubleScrapAudio, base.transform.position, 0.6f);
					}
					else
					{
						bool flag4 = outcome == GambleConstants.GamblingOutcome.HALVE;
						if (flag4)
						{
							AudioSource.PlayClipAtPoint(Plugin.GamblingHalveScrapAudio, base.transform.position, 0.6f);
						}
						else
						{
							bool flag5 = outcome == GambleConstants.GamblingOutcome.REMOVE;
							if (flag5)
							{
								AudioSource.PlayClipAtPoint(Plugin.GamblingRemoveScrapAudio, base.transform.position, 0.6f);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000393E File Offset: 0x00001B3E
		public void PlayDrumRoll()
		{
			AudioSource.PlayClipAtPoint(Plugin.GamblingDrumrollScrapAudio, base.transform.position, 0.6f);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000395C File Offset: 0x00001B5C
		public void BeginGamblingMachineCooldown(Action onCountdownFinish)
		{
			this.SetCurrentGamblingCooldownToMaxCooldown();
			bool flag = this.CountdownCooldownCoroutineBeingRan != null;
			if (flag)
			{
				base.StopCoroutine(this.CountdownCooldownCoroutineBeingRan);
			}
			this.CountdownCooldownCoroutineBeingRan = base.StartCoroutine(this.CountdownCooldownCoroutine(onCountdownFinish));
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000039A0 File Offset: 0x00001BA0
		public bool isInCooldownPhase()
		{
			return this.gamblingMachineCurrentCooldown > 0;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000039BB File Offset: 0x00001BBB
		private IEnumerator CountdownCooldownCoroutine(Action onCountdownFinish)
		{
			GamblingMachine.<CountdownCooldownCoroutine>d__29 <CountdownCooldownCoroutine>d__ = new GamblingMachine.<CountdownCooldownCoroutine>d__29(0);
			<CountdownCooldownCoroutine>d__.<>4__this = this;
			<CountdownCooldownCoroutine>d__.onCountdownFinish = onCountdownFinish;
			return <CountdownCooldownCoroutine>d__;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000039D1 File Offset: 0x00001BD1
		public void SetCurrentGamblingCooldownToMaxCooldown()
		{
			this.gamblingMachineCurrentCooldown = this.gamblingMachineMaxCooldown;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000039E0 File Offset: 0x00001BE0
		public void SetRoll(int newRoll)
		{
			this.currentRoll = newRoll;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000039EC File Offset: 0x00001BEC
		public int RollDice()
		{
			int num = Random.Range(this.rollMinValue, this.rollMaxValue);
			Plugin.mls.LogMessage(string.Format("rollMinValue: {0}", this.rollMinValue));
			Plugin.mls.LogMessage(string.Format("rollMaxValue: {0}", this.rollMaxValue));
			Plugin.mls.LogMessage(string.Format("Roll value: {0}", this.currentRoll));
			return num;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003A74 File Offset: 0x00001C74
		public int GetScrapValueBasedOnGambledOutcome(GrabbableObject scrap)
		{
			return (int)Mathf.Floor((float)scrap.scrapValue * this.currentGamblingOutcomeMultiplier);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003A9C File Offset: 0x00001C9C
		[ServerRpc(RequireOwnership = false)]
		public void ActivateGamblingMachineServerRPC(NetworkBehaviourReference scrapBeingGambledRef, NetworkBehaviourReference playerWhoGambledRef, ServerRpcParams serverRpcParams = default(ServerRpcParams))
		{
			NetworkManager networkManager = base.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (this.__rpc_exec_stage != 1 && (networkManager.IsClient || networkManager.IsHost))
			{
				FastBufferWriter fastBufferWriter = base.__beginSendServerRpc(3977934568U, serverRpcParams, 0);
				fastBufferWriter.WriteValueSafe<NetworkBehaviourReference>(ref scrapBeingGambledRef, default(FastBufferWriter.ForNetworkSerializable));
				fastBufferWriter.WriteValueSafe<NetworkBehaviourReference>(ref playerWhoGambledRef, default(FastBufferWriter.ForNetworkSerializable));
				base.__endSendServerRpc(ref fastBufferWriter, 3977934568U, serverRpcParams, 0);
			}
			if (this.__rpc_exec_stage != 1 || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			this.__rpc_exec_stage = 0;
			bool flag = !base.IsServer;
			if (!flag)
			{
				bool flag2 = this.numberOfUses <= 0;
				if (flag2)
				{
					Plugin.mls.LogWarning("ActivateGamblingMachineServerRPC: Machine usage limit has been reached");
				}
				else
				{
					bool flag3 = this.lockGamblingMachineServer;
					if (flag3)
					{
						Plugin.mls.LogWarning(string.Format("Gambling machine is already processing one client's request. Throwing away a request for... {0}", serverRpcParams.Receive.SenderClientId));
					}
					else
					{
						this.lockGamblingMachineServer = true;
						this.numberOfUses--;
						Plugin.mls.LogInfo(string.Format("ActivateGamblingMachineServerRPC: Number of uses left: {0}", this.numberOfUses));
						GrabbableObject grabbableObject;
						bool flag4 = !scrapBeingGambledRef.TryGet<GrabbableObject>(ref grabbableObject, null);
						if (flag4)
						{
							Plugin.mls.LogError("ActivateGamblingMachineServerRPC: Failed to get scrap value on client side.");
						}
						else
						{
							this.BeginGamblingMachineCooldownClientRpc();
							Plugin.mls.LogMessage("ActivateGamblingMachineServerRPC: Starting gambling machine cooldown phase in the server invoked by: " + serverRpcParams.Receive.SenderClientId.ToString());
							this.SetRoll(this.RollDice());
							this.GenerateGamblingOutcomeFromCurrentRoll();
							int scrapValueBasedOnGambledOutcome = this.GetScrapValueBasedOnGambledOutcome(grabbableObject);
							this.ActivateGamblingMachineClientRPC(scrapBeingGambledRef, playerWhoGambledRef, serverRpcParams.Receive.SenderClientId, scrapValueBasedOnGambledOutcome, this.currentGamblingOutcome, this.numberOfUses);
						}
					}
				}
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003CDC File Offset: 0x00001EDC
		[ClientRpc]
		private void ActivateGamblingMachineClientRPC(NetworkBehaviourReference scrapBeingGambledRef, NetworkBehaviourReference playerWhoGambledRef, ulong invokerId, int updatedScrapValue, string outcome, int numberOfUsesServer)
		{
			NetworkManager networkManager = base.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			NetworkBehaviourReference scrapBeingGambledRef;
			int updatedScrapValue;
			string outcome;
			if (this.__rpc_exec_stage != 1 && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams;
				FastBufferWriter fastBufferWriter = base.__beginSendClientRpc(875756295U, clientRpcParams, 0);
				fastBufferWriter.WriteValueSafe<NetworkBehaviourReference>(ref scrapBeingGambledRef, default(FastBufferWriter.ForNetworkSerializable));
				fastBufferWriter.WriteValueSafe<NetworkBehaviourReference>(ref playerWhoGambledRef, default(FastBufferWriter.ForNetworkSerializable));
				BytePacker.WriteValueBitPacked(fastBufferWriter, invokerId);
				BytePacker.WriteValueBitPacked(fastBufferWriter, updatedScrapValue);
				bool flag = outcome != null;
				fastBufferWriter.WriteValueSafe<bool>(ref flag, default(FastBufferWriter.ForPrimitives));
				if (flag)
				{
					fastBufferWriter.WriteValueSafe(outcome, false);
				}
				BytePacker.WriteValueBitPacked(fastBufferWriter, numberOfUsesServer);
				base.__endSendClientRpc(ref fastBufferWriter, 875756295U, clientRpcParams, 0);
			}
			if (this.__rpc_exec_stage != 1 || (!networkManager.IsClient && !networkManager.IsHost))
			{
				return;
			}
			this.__rpc_exec_stage = 0;
			scrapBeingGambledRef = scrapBeingGambledRef;
			updatedScrapValue = updatedScrapValue;
			GamblingMachine <>4__this = this;
			outcome = outcome;
			Plugin.mls.LogInfo("ActivateGamblingMachineClientRPC: Activiating gambling machines on client...");
			this.numberOfUses = numberOfUsesServer;
			Plugin.mls.LogInfo(string.Format("ActivateGamblingMachineClientRPC: Number of uses left: {0}", this.numberOfUses));
			PlayerControllerCustom playerWhoGambled;
			bool flag2 = !playerWhoGambledRef.TryGet<PlayerControllerCustom>(ref playerWhoGambled, null);
			if (flag2)
			{
				Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get player who gambled.");
			}
			else
			{
				playerWhoGambled.LockGamblingMachine();
				this.PlayDrumRoll();
				this.BeginGamblingMachineCooldown(delegate
				{
					GrabbableObject grabbableObject;
					bool flag3 = !scrapBeingGambledRef.TryGet<GrabbableObject>(ref grabbableObject, null);
					if (flag3)
					{
						Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get scrap value on client side.");
					}
					else
					{
						Plugin.mls.LogInfo(string.Format("Setting scrap value to: {0}", updatedScrapValue));
						grabbableObject.SetScrapValue(updatedScrapValue);
						<>4__this.PlayGambleResultAudio(outcome);
						playerWhoGambled.ReleaseGamblingMachineLock();
						bool isServer = <>4__this.IsServer;
						if (isServer)
						{
							Plugin.mls.LogMessage("Unlocking gambling machine");
							<>4__this.lockGamblingMachineServer = false;
						}
					}
				});
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003EF8 File Offset: 0x000020F8
		[ClientRpc]
		private void BeginGamblingMachineCooldownClientRpc()
		{
			NetworkManager networkManager = base.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (this.__rpc_exec_stage != 1 && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams;
				FastBufferWriter fastBufferWriter = base.__beginSendClientRpc(1546410253U, clientRpcParams, 0);
				base.__endSendClientRpc(ref fastBufferWriter, 1546410253U, clientRpcParams, 0);
			}
			if (this.__rpc_exec_stage != 1 || (!networkManager.IsClient && !networkManager.IsHost))
			{
				return;
			}
			this.__rpc_exec_stage = 0;
			Plugin.mls.LogInfo("Setting machine cooldown to max");
			this.SetCurrentGamblingCooldownToMaxCooldown();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003FE4 File Offset: 0x000021E4
		private void InitAudioSource()
		{
			bool flag = !this.isMusicEnabled;
			if (flag)
			{
				base.GetComponent<AudioSource>().Pause();
			}
			base.GetComponent<AudioSource>().volume = this.musicVolume;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004074 File Offset: 0x00002274
		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000408C File Offset: 0x0000228C
		protected override void __initializeRpcs()
		{
			base.__registerRpc(3977934568U, new NetworkBehaviour.RpcReceiveHandler(GamblingMachine.__rpc_handler_3977934568), "ActivateGamblingMachineServerRPC");
			base.__registerRpc(875756295U, new NetworkBehaviour.RpcReceiveHandler(GamblingMachine.__rpc_handler_875756295), "ActivateGamblingMachineClientRPC");
			base.__registerRpc(1546410253U, new NetworkBehaviour.RpcReceiveHandler(GamblingMachine.__rpc_handler_1546410253), "BeginGamblingMachineCooldownClientRpc");
			base.__initializeRpcs();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000040F8 File Offset: 0x000022F8
		private static void __rpc_handler_3977934568(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			NetworkBehaviourReference networkBehaviourReference;
			reader.ReadValueSafe<NetworkBehaviourReference>(ref networkBehaviourReference, default(FastBufferWriter.ForNetworkSerializable));
			NetworkBehaviourReference networkBehaviourReference2;
			reader.ReadValueSafe<NetworkBehaviourReference>(ref networkBehaviourReference2, default(FastBufferWriter.ForNetworkSerializable));
			ServerRpcParams server = rpcParams.Server;
			target.__rpc_exec_stage = 1;
			((GamblingMachine)target).ActivateGamblingMachineServerRPC(networkBehaviourReference, networkBehaviourReference2, server);
			target.__rpc_exec_stage = 0;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004198 File Offset: 0x00002398
		private static void __rpc_handler_875756295(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			NetworkBehaviourReference networkBehaviourReference;
			reader.ReadValueSafe<NetworkBehaviourReference>(ref networkBehaviourReference, default(FastBufferWriter.ForNetworkSerializable));
			NetworkBehaviourReference networkBehaviourReference2;
			reader.ReadValueSafe<NetworkBehaviourReference>(ref networkBehaviourReference2, default(FastBufferWriter.ForNetworkSerializable));
			ulong num;
			ByteUnpacker.ReadValueBitPacked(reader, ref num);
			int num2;
			ByteUnpacker.ReadValueBitPacked(reader, ref num2);
			bool flag;
			reader.ReadValueSafe<bool>(ref flag, default(FastBufferWriter.ForPrimitives));
			string text = null;
			if (flag)
			{
				reader.ReadValueSafe(ref text, false);
			}
			int num3;
			ByteUnpacker.ReadValueBitPacked(reader, ref num3);
			target.__rpc_exec_stage = 1;
			((GamblingMachine)target).ActivateGamblingMachineClientRPC(networkBehaviourReference, networkBehaviourReference2, num, num2, text, num3);
			target.__rpc_exec_stage = 0;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004298 File Offset: 0x00002498
		private static void __rpc_handler_1546410253(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			target.__rpc_exec_stage = 1;
			((GamblingMachine)target).BeginGamblingMachineCooldownClientRpc();
			target.__rpc_exec_stage = 0;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000042E9 File Offset: 0x000024E9
		protected internal override string __getTypeName()
		{
			return "GamblingMachine";
		}

		// Token: 0x04000025 RID: 37
		private int gamblingMachineMaxCooldown;

		// Token: 0x04000026 RID: 38
		public int gamblingMachineCurrentCooldown = 0;

		// Token: 0x04000027 RID: 39
		private float jackpotMultiplier;

		// Token: 0x04000028 RID: 40
		private float tripleMultiplier;

		// Token: 0x04000029 RID: 41
		private float doubleMultiplier;

		// Token: 0x0400002A RID: 42
		private float halvedMultiplier;

		// Token: 0x0400002B RID: 43
		private float zeroMultiplier;

		// Token: 0x0400002C RID: 44
		private int jackpotPercentage;

		// Token: 0x0400002D RID: 45
		private int triplePercentage;

		// Token: 0x0400002E RID: 46
		private int doublePercentage;

		// Token: 0x0400002F RID: 47
		private int halvedPercentage;

		// Token: 0x04000030 RID: 48
		private int removedPercentage;

		// Token: 0x04000031 RID: 49
		private bool isMusicEnabled = true;

		// Token: 0x04000032 RID: 50
		private float musicVolume = 0.35f;

		// Token: 0x04000033 RID: 51
		private int rollMinValue;

		// Token: 0x04000034 RID: 52
		private int rollMaxValue;

		// Token: 0x04000035 RID: 53
		private int currentRoll = 100;

		// Token: 0x04000036 RID: 54
		public float currentGamblingOutcomeMultiplier = 1f;

		// Token: 0x04000037 RID: 55
		public string currentGamblingOutcome = GambleConstants.GamblingOutcome.DEFAULT;

		// Token: 0x04000038 RID: 56
		private Coroutine CountdownCooldownCoroutineBeingRan;

		// Token: 0x04000039 RID: 57
		private bool lockGamblingMachineServer = false;

		// Token: 0x0400003A RID: 58
		public int numberOfUses;
	}
}
