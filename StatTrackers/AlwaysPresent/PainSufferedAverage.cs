using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class PainSufferedAverage : StatGeneric<float> {
        public override string name => "PainSufferedAverage";
        public override int priority => 0;
        private float deltatime = 0;
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

        public override void LoadToStatic() {
            value_running = value;
            deltatime_running = deltatime;
        }

        protected override void LoadFromStatic() {
            value = value_running;
            deltatime_running = deltatime;
        }
        
        public override string Serialize() {
            LoadFromStatic();
            try {
                Dictionary<string, string> data = new Dictionary<string, string>() {
                    {"value", JsonConvert.SerializeObject(value)},
                    {"deltatime", JsonConvert.SerializeObject(deltatime)},
                };
                return JsonConvert.SerializeObject(data);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to serialize object.\nName: {name}\nValue: {value}");
                throw;
            }
        }
        
        public override void Deserialize(string serialized) {
            try {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized);
                value = JsonConvert.DeserializeObject<float>(data["value"]);
                deltatime = JsonConvert.DeserializeObject<float>(data["deltatime"]);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}