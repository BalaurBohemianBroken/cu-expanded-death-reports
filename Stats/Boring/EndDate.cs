using System;
using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class EndDate : IStat {
        public string name => "EndDate";
        public string fieldName => "DATE: ";

        private string timeRunning => DateTime.Now.ToString("MM-dd-HH-mm-ss");
        private string time;

        public string GetStatReadout(bool color) {
            // TODO: This is not a perfect solution, but it works for now. Check when codebase is more finished that this still works.
            time ??= timeRunning;
            return "2XXX-" + time;
        }

        public void Reset() {
            time = "";
        }

        public float Noteworthiness() {
            return 0;
        }

        public string Serialize() {
            string t = timeRunning;
            try {
                return JsonConvert.SerializeObject(t);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to serialize object.\nName: {name}\nValue: {t}");
                throw;
            }
        }
        
        public virtual void Deserialize(string serialized) {
            try {
                time = JsonConvert.DeserializeObject<string>(serialized);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}