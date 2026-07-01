using System;
using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class RunTime : IStat {
        public string name => GetType().Name;
        public string fieldName => "DESCENT DURATION: ";

        private string valueRunning => TimeSpan.FromSeconds(WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss");
        private string valueStored = null;

        public float Noteworthiness() {
            return 0;
        }

        public string GetStatReadout(bool color) {
            var instance = IStat.Get<RunTime>();
            // TODO: Check this works.
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