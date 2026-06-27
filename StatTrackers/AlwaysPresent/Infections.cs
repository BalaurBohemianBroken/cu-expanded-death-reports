using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    public class Infections : StatGeneric<int> {
        public override string name => "Infections";
        public override int priority => 0;
        public static bool was_infect_last_check = false;
        
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            if (!__instance.infected)
                was_infect_last_check = false;
            else {
                if (!was_infect_last_check) {
                    value_running += 1;
                }
                was_infect_last_check = true;
            }
        }

        public override bool IsNoteworthy() {
            return false;
        }
    }
}