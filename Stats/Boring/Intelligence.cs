using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Intelligence : IStat {
        public string name => "Intelligence";
        public string fieldName => "INTELLIGENCE: ";

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

        public string GetStatReadout(bool color) {
            var instance = IStat.Get<Intelligence>();
            string s;
            // TODO: Check this works.
            if (this == instance)
                s = valueRunning.ToString();
            else 
                s = valueStored.ToString();
            
            if (color)
                s = DeathReport.ColorVar(s);
            
            return s + " INT";
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