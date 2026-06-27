using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Amputations : StatGeneric<int> {
        public override string name => "CutLimbsOff";
        public override int priority => 0;
        
        private List<string> _notes = new List<string>() {
            "Impressive. How might we teach this survival behaviour?",
            "What the fuck are we doing?\nResearch.",
        };
        protected override List<string> notes => _notes;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AmputationMinigame), nameof(AmputationMinigame.Update))]
        public static void PatchUpdate(AmputationMinigame __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.limb.body))
                return;
            if (__instance.cutProgress >= 1) {
                value_running += 1;
            }
        }

        public override bool IsNoteworthy() {
            return value > 0;
        }
    }
}