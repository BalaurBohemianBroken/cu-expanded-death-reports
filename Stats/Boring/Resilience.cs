using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Resilience : IStat { 
        private static Resilience instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (Resilience)value;
        }
        public string name => "Resilience";
        public int priority => 0;
        public string fieldName => "RESILIENCE: ";

        public int valueRunning => PlayerCamera.main?.body?.skills?.RES ?? 0;
        private int valueStored = 0;

        private List<string> _notes_positive = new List<string> {
            "Indomitable.",
            "Endurance, paid for in blood.",
        };
        private List<string> _notes_negative = new List<string> {
            "A soft heart, not made for this..."
        };
        
        public float Noteworthiness() {
            return 0;
        }

        public string GetStatReadout(bool color) {
            string s;
            if (this == instance)
                s = valueRunning.ToString();
            else
                s = valueStored.ToString();
            if (color)
                s = DeathReport.ColorVar(s);
            return s;
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