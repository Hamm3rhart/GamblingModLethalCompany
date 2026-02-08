using System;
using HarmonyLib;

namespace GamblersMod.Patches
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(StartOfRound))]
	internal class StartOfRoundPatch
	{
		// Token: 0x06000029 RID: 41 RVA: 0x0000303F File Offset: 0x0000123F
		[HarmonyPatch("Awake")]
		[HarmonyPostfix]
		private static void AwakePatch(StartOfRound __instance)
		{
			Plugin.mls.LogInfo("StartOfRoundPatch has awoken");
		}
	}
}
