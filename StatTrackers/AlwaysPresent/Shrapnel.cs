using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Shrapnel : StatGeneric<int> {
        public override string name => "Shrapnel";
        public override int priority => 0;
        
        private static Dictionary<Limb, int> shrapnelLastFrame = new();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            int last_shrapnel = shrapnelLastFrame.GetValueOrDefault(__instance, 0);
            if (__instance.shrapnel > last_shrapnel) {
                instance.value += __instance.shrapnel - last_shrapnel;
            }
            shrapnelLastFrame[__instance] = last_shrapnel;
        }

        public override void Reset() {
            value = 0;
            if (this == instance)
                shrapnelLastFrame = new Dictionary<Limb, int>();
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}