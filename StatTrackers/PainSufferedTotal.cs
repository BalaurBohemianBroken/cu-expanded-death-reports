using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class PainSufferedTotal : StatGeneric<float> {
        public override string name => "PainSufferedTotal";
        public override int priority => 0;

        private static float noteworthyThreshold = 25;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), nameof(Body.Update))]
        public static void PatchUpdate(Body __instance) {
            // This might be wrong if they do their own timescaling anywhere. They might do this when you're reading notes?
            // But I'm hoping they use unity's builtin functions.
            if (!ExpandedDeathReports.IsMainBody(__instance))
                return;
            instance.value += CalculatePainUnits(__instance.averagePain, Time.deltaTime);
        }

        public override float Noteworthiness() {
            // TODO: Scale with biome.
            if (value >= noteworthyThreshold)
                return 1;
            return 0;
        }

        private static float CalculatePainUnits(float pain_level, float seconds) {
            // 1 pain unit is 1 hour at 1 pain level
            return pain_level * (seconds / 60 / 60);
        }
    }
}