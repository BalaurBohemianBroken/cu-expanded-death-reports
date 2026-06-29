using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Intelligence : IStat {
        private static Intelligence instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (Intelligence)value;
        }
        public string name => "Intelligence";
        public int priority => 0;

        private int valueRunning => PlayerCamera.main?.body?.skills?.INT ?? 0;
        private int valueStored = 0;

        private List<string> _notes_positive = new() {
            "Smart little beast...",
            "What a waste.",
        };
        private List<string> _notes_negative = new() {
            "How did they make it this far?"
        };

        public float Noteworthiness() {
            return 0;
        }

        public string GetStatReadout() {
            // TODO: Check this works.
            if (this == instance)
                return valueRunning + " INT";
            return valueStored + " INT";
        }

        public void Reset() {
            valueStored = 0;
        }

        public virtual string Serialize() {
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
                valueStored = JsonConvert.DeserializeObject<int>(serialized);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}