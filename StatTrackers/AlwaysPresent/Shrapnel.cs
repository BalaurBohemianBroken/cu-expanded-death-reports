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
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        private static Dictionary<Limb, int> shrapnel_last_frame = new Dictionary<Limb, int>();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            int last_shrapnel = shrapnel_last_frame.GetValueOrDefault(__instance, 0);
            if (__instance.shrapnel > last_shrapnel) {
                value_running += __instance.shrapnel - last_shrapnel;
            }
            shrapnel_last_frame[__instance] = last_shrapnel;
        }

        public override bool IsNoteworthy() {
            return false;
        }
    }
}