using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Strength : IStat {
        private static Strength instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (Strength)value;
        }
        public string name => "Strength";
        public int priority => 0;

        private int valueRunning => PlayerCamera.main?.body?.skills?.STR ?? 0;
        private int valueStored = 0;

        public bool IsNoteworthy() {
            return false;
        }

        public string GetStatReadout(int decimal_place = -1) {
            if (this == instance)
                return valueRunning + " STR";
            return valueStored + " STR";
        }

        public void Reset() {
            valueStored = 0;
        }

        public string Serialize() {
            // Not sure if there's a situation where I would want to serialize a stored value rather than a live value.
            // If so, I'll need to handle that.
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