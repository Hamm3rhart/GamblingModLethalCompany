using System;
using GamblersMod.Patches;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamblersMod.Player
{
	// Token: 0x02000006 RID: 6
	internal class PlayerControllerCustom : NetworkBehaviour
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002C1D File Offset: 0x00000E1D
		private void Awake()
		{
			this.PlayerGamblingUIManager = base.gameObject.AddComponent<PlayerGamblingUIManager>();
			this.PlayerControllerOriginal = base.gameObject.GetComponent<PlayerControllerB>();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002C44 File Offset: 0x00000E44
		private void Update()
		{
			bool flag = !base.IsOwner;
			if (!flag)
			{
				Camera gameplayCamera = this.PlayerControllerOriginal.gameplayCamera;
				Vector3 position = gameplayCamera.transform.position;
				Vector3 forward = gameplayCamera.transform.forward;
				Ray ray;
				ray..ctor(position, forward);
				float num = 5f;
				int num2 = 512;
				RaycastHit raycastHit;
				bool flag2 = Physics.Raycast(ray, ref raycastHit, num, num2);
				bool flag3 = raycastHit.collider;
				if (flag3)
				{
					GameObject gameObject = raycastHit.transform.gameObject;
					bool flag4 = gameObject.name.Contains("GamblingMachine");
					if (flag4)
					{
						this.PlayerGamblingUIManager.ShowInteractionText();
						GrabbableObject grabbableObject = this.PlayerControllerOriginal.ItemSlots[this.PlayerControllerOriginal.currentItemSlot];
						GamblingMachine component = gameObject.GetComponent<GamblingMachine>();
						bool flag5 = component.isInCooldownPhase();
						if (flag5)
						{
							this.PlayerGamblingUIManager.SetInteractionText(string.Format("Cooling down... {0}", component.gamblingMachineCurrentCooldown));
						}
						else
						{
							bool flag6 = component.numberOfUses <= 0;
							if (flag6)
							{
								this.PlayerGamblingUIManager.SetInteractionText("This machine is all used up");
							}
							else
							{
								string bindingDisplayString = InputActionRebindingExtensions.GetBindingDisplayString(IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact", false), 0, 0);
								bool flag7 = this.isUsingGamblingMachine;
								if (flag7)
								{
									this.PlayerGamblingUIManager.SetInteractionText("You're already using a machine");
								}
								else
								{
									this.PlayerGamblingUIManager.SetInteractionText("Gamble: [" + bindingDisplayString + "]");
								}
							}
						}
						bool flag8 = grabbableObject;
						if (flag8)
						{
							this.PlayerGamblingUIManager.SetInteractionSubText(string.Format("Scrap value on hand: ■{0}", grabbableObject.scrapValue));
						}
						else
						{
							this.PlayerGamblingUIManager.SetInteractionSubText("Please hold a scrap on your hand");
						}
					}
					bool flag9 = gameObject.name.Contains("GamblingMachine") && IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact", false).triggered;
					if (flag9)
					{
						GamblingMachine component2 = gameObject.GetComponent<GamblingMachine>();
						this.handleGamblingMachineInput(component2);
					}
				}
				else
				{
					this.PlayerGamblingUIManager.HideInteractionText();
				}
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002E87 File Offset: 0x00001087
		public void ReleaseGamblingMachineLock()
		{
			Plugin.mls.LogInfo(string.Format("Releasing gambling machine lock for: {0}", base.OwnerClientId));
			this.isUsingGamblingMachine = false;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002EB1 File Offset: 0x000010B1
		public void LockGamblingMachine()
		{
			Plugin.mls.LogInfo(string.Format("Locking gambling machine for: {0}", base.OwnerClientId));
			this.isUsingGamblingMachine = true;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002EDC File Offset: 0x000010DC
		private void handleGamblingMachineInput(GamblingMachine GamblingMachineHit)
		{
			GrabbableObject grabbableObject = this.PlayerControllerOriginal.ItemSlots[this.PlayerControllerOriginal.currentItemSlot];
			bool flag = !grabbableObject;
			if (!flag)
			{
				bool flag2 = GamblingMachineHit.isInCooldownPhase() || GamblingMachineHit.numberOfUses <= 0 || this.isUsingGamblingMachine;
				if (!flag2)
				{
					Plugin.mls.LogInfo("Gambling machine was interacted with by: " + this.PlayerControllerOriginal.playerUsername);
					GamblingMachineHit.SetCurrentGamblingCooldownToMaxCooldown();
					Plugin.mls.LogMessage(string.Format("Scrap value of {0} on hand: ▊{1}", grabbableObject.name, grabbableObject.scrapValue));
					GamblingMachineHit.ActivateGamblingMachineServerRPC(grabbableObject, this, default(ServerRpcParams));
					this.PlayerGamblingUIManager.SetInteractionText(string.Format("Cooling down... {0}", GamblingMachineHit.gamblingMachineCurrentCooldown));
				}
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002FC4 File Offset: 0x000011C4
		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002FDA File Offset: 0x000011DA
		protected override void __initializeRpcs()
		{
			base.__initializeRpcs();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002FE4 File Offset: 0x000011E4
		protected internal override string __getTypeName()
		{
			return "PlayerControllerCustom";
		}

		// Token: 0x0400001A RID: 26
		private PlayerGamblingUIManager PlayerGamblingUIManager;

		// Token: 0x0400001B RID: 27
		private PlayerControllerB PlayerControllerOriginal;

		// Token: 0x0400001C RID: 28
		public bool isUsingGamblingMachine;
	}
}
