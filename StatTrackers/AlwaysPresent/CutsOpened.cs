using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class CutsOpened : StatGeneric<int> {
        public override string name => "CutsOpened";
        public override int priority => 0;
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        private static Dictionary<Limb, bool> was_cut_last_frame = new Dictionary<Limb, bool>();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            // Because I'm not clearing this value, theoretically cuts could be carried over between runs.
            // But I really can't see it.
            bool is_cut = __instance.bleedAmount > 0;
            if (is_cut && !was_cut_last_frame.GetValueOrDefault(__instance, true))
                value_running += 1;
            was_cut_last_frame[__instance] = __instance.bleedAmount > 0;
        }

        public override bool IsNoteworthy() {
            return false;
        }
    }
}