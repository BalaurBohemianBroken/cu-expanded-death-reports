using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class Strength : IStat {
        public string name => "Strength";
        public int priority => 0;

        public int value_running => PlayerCamera.main?.body?.skills?.STR ?? 0;
        private int value = 0;

        private List<string> _notes_positive = new List<string> {
        };
        private List<string> _notes_negative = new List<string> {
        };

        public bool IsNoteworthy() {
            return false;
        }

        public void LoadToStatic() {
            // No. Don't do this. It's a mechanic controlled by the game.
            return;
        }

        public void LoadFromStatic() {
            value = value_running;
        }

        public string GetValue(int decimal_place = -1) {
            return value.ToString();
        }
        
        public virtual string Serialize() {
            LoadFromStatic();
            try {
                return JsonConvert.SerializeObject(value);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to serialize object.\nName: {name}\nValue: {value}");
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