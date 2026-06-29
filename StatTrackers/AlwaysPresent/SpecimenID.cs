using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class SpecimenID : IStat {
        public string name => "SpecimenID";
        public IStat runningInstance { get; set; }
        public int priority => 0;

        private int valueRunning => WoundView.specimenId;
        private int value;

        public string GetStatReadout() {
            // TODO: This is not a perfect solution, but it works for now. Check when codebase is more finished that this still works.
            if (value == 0)
                value = valueRunning;
            return $"#{value}-SAW-01";
        }

        public void Reset() {
            value = 0;
        }

        public float Noteworthiness() {
            return 0;
        }

        public string Serialize() {
            try {
                return JsonConvert.SerializeObject(valueRunning);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to serialize object.\nName: {name}\nValue: {valueRunning}");
                throw;
            }
        }
        
        public virtual void Deserialize(string serialized) {
            try {
                value = JsonConvert.DeserializeObject<int>(serialized);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}