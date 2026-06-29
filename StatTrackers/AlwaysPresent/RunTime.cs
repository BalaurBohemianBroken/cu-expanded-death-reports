using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class RunTime : IStat {
        private static RunTime instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (RunTime)value;
        }
        public string name => "RunTime";
        public int priority => 0;

        private string valueRunning => TimeSpan.FromSeconds(WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss");
        private string valueStored = null;

        public bool IsNoteworthy() {
            return false;
        }

        public string GetStatReadout(int decimal_place = -1) {
            // TODO: Check this works.
            if (this == instance)
                return valueRunning.ToString();
            return valueStored.ToString();
        }

        public void Reset() {
            valueStored = null;
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
                valueStored = JsonConvert.DeserializeObject<string>(serialized);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}