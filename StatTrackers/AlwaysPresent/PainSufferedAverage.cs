using System;
using System.Collections.Generic;
using System.Globalization;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    // Because this one has a bunch of custom behaviour I've opted to not make it StatGeneric.
    [HarmonyPatch]
    public class PainSufferedAverage : IStat {
        private static PainSufferedAverage instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (PainSufferedAverage) value;
        }
        public string name => "PainSufferedAverage";
        public string fieldName => "PAIN AVG:";
        public int priority => 0;
        
        private float value = 0;
        private float deltatime = 0;

        private const float updateInterval = 1;
        private static float timeSinceLastUpdate = 0;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), nameof(Body.Update))]
        public static void PatchUpdate(Body __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance))
                return;
            timeSinceLastUpdate += Time.deltaTime;
            while (timeSinceLastUpdate > updateInterval) {
                timeSinceLastUpdate -= updateInterval;
                instance.value += __instance.averagePain;
                instance.deltatime += 1;
            }
        }

        public bool IsNoteworthy() {
            return false;
        }

        public float GetAveragePain() {
            return value / deltatime;
        }

        // TODO: This reports back NAN
        public string GetStatReadout(int decimal_place = -1) {
            float v = GetAveragePain();
            string s;
            if (decimal_place > -1)
                s = Math.Round(v, decimal_place).ToString(CultureInfo.InvariantCulture);
            else
                s = v.ToString(CultureInfo.InvariantCulture);

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