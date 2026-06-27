using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    public class PainSufferedAverage : StatGeneric<float> {
        public override string name => "PainSufferedAverage";
        public override int priority => 0;
        public static float deltatime_running = 0;

        private const float update_interval = 1;
        private static float time_since_last_update = 0;
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), nameof(Body.Update))]
        public static void PatchUpdate(Body __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance))
                return;
            time_since_last_update = Time.deltaTime;
            while (time_since_last_update > update_interval) {
                time_since_last_update -= update_interval;
                value_running += __instance.averagePain;
                deltatime_running += 1;
            }
        }

        public override bool IsNoteworthy() {
            return false;
        }

        public static float GetAveragePainRunning() {
            return value_running / deltatime_running;
        }
    }
}