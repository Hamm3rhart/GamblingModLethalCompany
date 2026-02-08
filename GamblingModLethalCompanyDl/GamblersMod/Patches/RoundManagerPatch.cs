using System;
using GamblersMod.RoundManagerCustomSpace;
using HarmonyLib;

namespace GamblersMod.Patches
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(RoundManager))]
	internal class RoundManagerPatch
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00004549 File Offset: 0x00002749
		[HarmonyPatch("Awake")]
		[HarmonyPostfix]
		public static void AwakePatch(RoundManager __instance)
		{
			Plugin.mls.LogInfo("RoundManagerPatch has awoken");
			RoundManagerPatch.RoundManagerCustom = __instance.gameObject.AddComponent<RoundManagerCustom>();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000456C File Offset: 0x0000276C
		[HarmonyPatch("LoadNewLevelWait")]
		[HarmonyPrefix]
		public static void LoadNewLevelWaitPatch(RoundManager __instance)
		{
			Plugin.mls.LogInfo("FinishGeneratingNewLevelServerRpcPatch was called");
			bool flag = __instance.currentLevel.levelID != 3;
			if (flag)
			{
				Plugin.mls.LogInfo("Despawning gambling machine...");
				RoundManagerPatch.RoundManagerCustom.DespawnGamblingMachineServerRpc();
			}
			bool flag2 = __instance.currentLevel.levelID == 3;
			if (flag2)
			{
				Plugin.mls.LogInfo("Spawning gambling machine...");
				RoundManagerPatch.RoundManagerCustom.SpawnGamblingMachineServerRpc();
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000045EC File Offset: 0x000027EC
		[HarmonyPatch("DespawnPropsAtEndOfRound")]
		[HarmonyPostfix]
		public static void DespawnPropsAtEndOfRoundPatch()
		{
			Plugin.mls.LogInfo("End of round: despawning gambling machines");
			RoundManagerPatch.RoundManagerCustom.DespawnGamblingMachineServerRpc();
		}

		// Token: 0x04000044 RID: 68
		public static RoundManagerCustom RoundManagerCustom;
	}
}
