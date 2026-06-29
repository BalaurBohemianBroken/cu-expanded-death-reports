using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Dislocations : StatGeneric<int> {
        public override string name => "Dislocations";
        public override int priority => 0;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.BreakBone))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            if (!__instance.dislocated)
                instance.value += 1;
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}