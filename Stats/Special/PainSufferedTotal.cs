using HarmonyLib;
using UnityEngine;

namespace BalaurBohemianBroken.Stats;

[HarmonyPatch]
public class PainSufferedTotal : StatGeneric<float> {
    public override string fieldName => "PAIN UNITS: ";
        
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Body), nameof(Body.Update))]
    public static void PatchUpdate(Body __instance) {
        // This might be wrong if they do their own timescaling anywhere. They might do this when you're reading notes?
        // But I'm hoping they use unity's builtin functions.
        if (!ExpandedDeathReports.IsMainBody(__instance))
            return;
        IStat.Get<PainSufferedTotal>().value += CalculatePainUnits(__instance.averagePain, Time.deltaTime);
    }

    public override float Noteworthiness() {
        // TODO: Scale with biome.
        if (value >= 25)
            return 1;
        return 0;
    }

    private static float CalculatePainUnits(float pain_level, float seconds) {
        // 1 pain unit is 1 hour at 1 pain level
        return pain_level * (seconds / 60 / 60);
    }
}