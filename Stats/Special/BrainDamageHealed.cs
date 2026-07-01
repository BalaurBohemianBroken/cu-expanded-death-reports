using HarmonyLib;
using UnityEngine;

namespace BalaurBohemianBroken.Stats;

[HarmonyPatch]
public class BrainDamageHealed : StatGeneric<float> {
    public override string name => GetType().ToString();
    public override string fieldName => "HEALED (BRAIN): ";
    private float last_brain_health = -1;
    
    // Do not include the healing gained from last stand in this.
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Body), nameof(Body.TryLastStand))]
    public static void HandleLastStandPre(Body __instance, float __state) {
        if (!ExpandedDeathReports.IsMainBody(__instance))
            return;
        __state = __instance.brainHealth;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Body), nameof(Body.TryLastStand))]
    public static void HandleLastStandPost(Body __instance, float __state) {
        var instance = IStat.Get<BrainDamageHealed>();
        // TODO: Test this.
        float bh = __instance.brainHealth;
        if (!Mathf.Approximately(__state, bh)) {
            instance.value -= __state - bh;
        }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Body), nameof(Body.Update))]
    public static void TrackBrainHealth(Body __instance) {
        var instance = IStat.Get<BrainDamageHealed>();
        float bh = __instance.brainHealth;
        BrainDamageHealed i = instance as BrainDamageHealed;
        if (Mathf.Approximately(i.last_brain_health, -1))
            i.last_brain_health = bh;
        if (i.last_brain_health < bh)
            i.value += bh - i.last_brain_health;
        i.last_brain_health = bh;
    }
    
    public override float Noteworthiness() {
        float n = 0;
        if (value >= 25)
            n += 1;
        else if (value >= 100)
            n += 3;
        n += 10000;
        return n;
    }
}
