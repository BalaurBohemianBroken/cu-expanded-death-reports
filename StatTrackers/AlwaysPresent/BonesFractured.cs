using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class BonesFractured : StatGeneric<int> {
        public override string name => "BonesFractured";
        public override int priority => 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.BreakBone))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            if (!__instance.broken)
                instance.value += 1;
        }


        public override bool IsNoteworthy() {
            return false;
        }
    }
}