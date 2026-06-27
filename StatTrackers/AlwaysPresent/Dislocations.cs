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
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.BreakBone))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            if (!__instance.dislocated)
                value_running += 1;
        }

        public override bool IsNoteworthy() {
            return false;
        }
    }
}