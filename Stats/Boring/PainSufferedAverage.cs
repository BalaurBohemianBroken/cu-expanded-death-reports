using System;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;
using UnityEngine;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    // Because this one has a bunch of custom behaviour I've opted to not make it StatGeneric.
    [HarmonyPatch]
    public class PainSufferedAverage : IStat {
        public string name => GetType().Name;
        public string fieldName => "PAIN AVG:";
        
        private float value = 0;
        private float deltatime = 0;

        private const float updateInterval = 1;
        private float timeSinceLastUpdate = 0;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), nameof(Body.Update))]
        public static void PatchUpdate(Body __instance) {
            var instance = IStat.Get<PainSufferedAverage>();
            if (!ExpandedDeathReports.IsMainBody(__instance))
                return;
            instance.timeSinceLastUpdate += Time.deltaTime;
            while (instance.timeSinceLastUpdate > updateInterval) {
                instance.timeSinceLastUpdate -= updateInterval;
                instance.value += __instance.averagePain;
                instance.deltatime += 1;
            }
        }
        public float Noteworthiness() {
            return 0;
        }

        public float GetAveragePain() {
            return value / deltatime;
        }

        // TODO: This reports back NAN
        public string GetStatReadout(bool color) {
            float v = GetAveragePain();
            string s = Math.Round(v, 0).ToString(CultureInfo.InvariantCulture);
            if (color)
                s = DeathReport.ColorVar(s);
            return s + "%";
        }

        public void Reset() {
            value = 0;
            deltatime = 0;
        }
        
        public string Serialize() {
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
        
        public void Deserialize(string serialized) {
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