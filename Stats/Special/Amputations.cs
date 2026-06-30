using HarmonyLib;
using System.Collections.Generic;
using BalaurBohemianBroken.Stats;

namespace BalaurBohemianBroken;
[HarmonyPatch]
public class Amputations : StatGeneric<int> {
    public override string name => "CutLimbsOff";
    public override int priority => 0;
    public override string fieldName => "AMPUTATIONS: ";

    private List<string> _notes = new List<string>() {
        "Impressive. How might we teach this survival behaviour?",
        "What the fuck are we doing?\nResearch.",
    };
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmputationMinigame), nameof(AmputationMinigame.Update))]
    public static void PatchUpdate(AmputationMinigame __instance) {
        if (!ExpandedDeathReports.IsMainBody(__instance.limb.body))
            return;
        if (__instance.cutProgress >= 1) {
            instance.value += 1;
        }
    }

    public override float Noteworthiness() {
        if (value == 0)
            return 0;
        return 5;
    }
}