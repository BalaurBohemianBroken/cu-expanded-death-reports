using HarmonyLib;
using UnityEngine;

namespace BalaurBohemianBroken;

[HarmonyPatch]
public class HandleDeathScreenPatch {
    [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.HandleDeathScreen))]
    public static bool Prefix(PlayerCamera __instance) {
        if (!__instance.body.alive && __instance.blackAmount >= 1.0 && !__instance.didDeathScreen) {
            __instance.didDeathScreen = true;
            __instance.finishCoroutine = __instance.StartCoroutine(__instance.EndSequence(0));
        }
        if (!__instance.didDeathScreen || !__instance.body.alive)
            return false;
            
        if (__instance.finishCoroutine != null)
            __instance.StopCoroutine(__instance.finishCoroutine);
        __instance.endScreen.gameObject.SetActive(false);
        // Modified code here.
        Object.Destroy(EndScreenPatch.reportInstance.deathStats.gameObject);
        EndScreenPatch.reportInstance = null;
        
        if ((bool)(Object)PauseHandler.main)
            PauseHandler.main.ResetQuitPressed();
        __instance.didDeathScreen = false;
        // __instance.deathStats.anchoredPosition = new Vector2(-336f, -1000f);
        return false;
    }
}