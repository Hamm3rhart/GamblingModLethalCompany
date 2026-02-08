using System.Collections;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        //public static StartOfRoundCustom StartOfRoundCustom;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch(StartOfRound __instance)
        {
            Plugin.mls.LogInfo("StartOfRoundPatch has awoken");
            // Ensure message relays are registered once Netcode is fully initialized
            GambleRequestRelay.Register();
            GambleResultRelay.Register();
            __instance.StartCoroutine(RegisterRelaysWhenReady());
            //StartOfRoundCustom = __instance.gameObject.AddComponent<StartOfRoundCustom>();
        }

        private static IEnumerator RegisterRelaysWhenReady()
        {
            int attempts = 0;
            while (attempts < 20)
            {
                attempts++;
                var nm = NetworkManager.Singleton;
                if (nm != null && nm.CustomMessagingManager != null)
                {
                    GambleRequestRelay.Register();
                    GambleResultRelay.Register();
                    yield break;
                }

                yield return new WaitForSeconds(0.5f);
            }

            Plugin.LogDebug("[GambleRPC] Relay registration retry timed out");
        }
    }
}
