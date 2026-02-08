using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using GamblersMod.config;
using GamblersMod.Patches;
using HarmonyLib;
using UnityEngine;

namespace GamblersMod
{
	// Token: 0x02000003 RID: 3
	[BepInPlugin("Junypai.GamblersMod", "Gamblers Mod", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002218 File Offset: 0x00000418
		private void Awake()
		{
			bool flag = Plugin.Instance == null;
			if (flag)
			{
				Plugin.Instance = this;
			}
			Plugin.NetcodeWeaver();
			Plugin.mls = Logger.CreateLogSource("Junypai.GamblersMod");
			Plugin.CurrentUserConfig = new GambleConfigSettingsSerializable(base.Config);
			Plugin.RecentHostConfig = new GambleConfigSettingsSerializable(base.Config);
			Plugin.UserConfigSnapshot = new GambleConfigSettingsSerializable(base.Config);
			string directoryName = Path.GetDirectoryName(base.Info.Location);
			Plugin.mls.LogInfo("Loading gambler bundle assets");
			AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(directoryName, "gamblingmachinebundle"));
			bool flag2 = !assetBundle;
			if (flag2)
			{
				Plugin.mls.LogError("Unable to load gambler bundle assets");
			}
			else
			{
				Plugin.mls.LogInfo("Gamblers bundle assets successfully loaded");
			}
			Plugin.GamblingDrumrollScrapAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "drumroll");
			Plugin.GamblingJackpotScrapAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "holyshit");
			Plugin.GamblingHalveScrapAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "cricket");
			Plugin.GamblingRemoveScrapAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "womp");
			Plugin.GamblingMachineMusicAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "machineMusic");
			Plugin.GamblingDoubleScrapAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "doublekill");
			Plugin.GamblingTripleScrapAudio = this.LoadAssetFromAssetBundleAndLogInfo<AudioClip>(assetBundle, "triplekill");
			Plugin.GamblingFont = this.LoadAssetFromAssetBundleAndLogInfo<Font>(assetBundle, "3270-Regular");
			Plugin.GamblingMachine = this.LoadAssetFromAssetBundleAndLogInfo<GameObject>(assetBundle, "GamblingMachine");
			Plugin.GamblingHandIcon = this.LoadAssetFromAssetBundleAndLogInfo<GameObject>(assetBundle, "HandIconGO");
			Plugin.GamblingMachine.AddComponent<GamblingMachine>();
			new GameObject().AddComponent<GamblingMachineManager>();
			this.harmony.PatchAll(typeof(Plugin));
			this.harmony.PatchAll(typeof(GameNetworkManagerPatch));
			this.harmony.PatchAll(typeof(PlayerControllerBPatch));
			this.harmony.PatchAll(typeof(RoundManagerPatch));
			this.harmony.PatchAll(typeof(StartOfRoundPatch));
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002418 File Offset: 0x00000618
		private T LoadAssetFromAssetBundleAndLogInfo<T>(AssetBundle bundle, string assetName) where T : Object
		{
			T t = bundle.LoadAsset<T>(assetName);
			bool flag = !t;
			if (flag)
			{
				Plugin.mls.LogError(assetName + " asset failed to load");
			}
			else
			{
				Plugin.mls.LogInfo(assetName + " asset successfully loaded");
			}
			return t;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002478 File Offset: 0x00000678
		private static void NetcodeWeaver()
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
					bool flag = customAttributes.Length != 0;
					if (flag)
					{
						methodInfo.Invoke(null, null);
					}
				}
			}
		}

		// Token: 0x04000003 RID: 3
		public const string modGUID = "Junypai.GamblersMod";

		// Token: 0x04000004 RID: 4
		public const string modName = "Gamblers Mod";

		// Token: 0x04000005 RID: 5
		public const string modVersion = "1.0.0";

		// Token: 0x04000006 RID: 6
		private readonly Harmony harmony = new Harmony("Junypai.GamblersMod");

		// Token: 0x04000007 RID: 7
		public static Plugin Instance;

		// Token: 0x04000008 RID: 8
		public static GameObject GamblingMachine;

		// Token: 0x04000009 RID: 9
		public static AudioClip GamblingJackpotScrapAudio;

		// Token: 0x0400000A RID: 10
		public static AudioClip GamblingHalveScrapAudio;

		// Token: 0x0400000B RID: 11
		public static AudioClip GamblingRemoveScrapAudio;

		// Token: 0x0400000C RID: 12
		public static AudioClip GamblingDoubleScrapAudio;

		// Token: 0x0400000D RID: 13
		public static AudioClip GamblingTripleScrapAudio;

		// Token: 0x0400000E RID: 14
		public static AudioClip GamblingDrumrollScrapAudio;

		// Token: 0x0400000F RID: 15
		public static GameObject GamblingATMMachine;

		// Token: 0x04000010 RID: 16
		public static AudioClip GamblingMachineMusicAudio;

		// Token: 0x04000011 RID: 17
		public static GameObject GamblingMachineResultCanvas;

		// Token: 0x04000012 RID: 18
		public static Font GamblingFont;

		// Token: 0x04000013 RID: 19
		public static GameObject GamblingHandIcon;

		// Token: 0x04000014 RID: 20
		public static GambleConfigSettingsSerializable UserConfigSnapshot;

		// Token: 0x04000015 RID: 21
		public static GambleConfigSettingsSerializable RecentHostConfig;

		// Token: 0x04000016 RID: 22
		public static GambleConfigSettingsSerializable CurrentUserConfig;

		// Token: 0x04000017 RID: 23
		public static ManualLogSource mls;
	}
}
