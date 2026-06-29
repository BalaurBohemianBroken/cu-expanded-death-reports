using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Infections : StatGeneric<int> {
        public override string name => "Infections";
        public override int priority => 0;
        public static Dictionary<Limb, bool> infectionLastCheck = new();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            if (infectionLastCheck.GetValueOrDefault(__instance, false) && __instance.infected)
                instance.value += 1;
            infectionLastCheck[__instance] = __instance.infected;
        }

        public override void Reset() {
            value = 0;
            if (instance == this)
                infectionLastCheck = new Dictionary<Limb, bool>();
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}