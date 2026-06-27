using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    public class PainSufferedTotal : StatGeneric<float> {
        public override string name => "PainSufferedTotal";
        public override int priority => 0;

        private static float noteworthy_threshold = 25;
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), nameof(Body.Update))]
        public static void PatchUpdate(Body __instance) {
            // This might be wrong if they do their own timescaling anywhere. They might do this when you're reading notes?
            // But I'm hoping they use unity's builtin functions.
            if (!ExpandedDeathReports.IsMainBody(__instance))
                return;
            value_running += CalculatePainUnits(__instance.averagePain, Time.deltaTime);
        }

        public override bool IsNoteworthy() {
            return value > noteworthy_threshold;
        }

        private static float CalculatePainUnits(float pain_level, float seconds) {
            // 1 pain unit is 1 hour at 1 pain level
            return pain_level * (seconds / 60 / 60);
        }
    }
}