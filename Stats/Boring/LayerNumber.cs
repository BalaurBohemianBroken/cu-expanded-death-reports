using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class LayerNumber : IStat {
        private static LayerNumber instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (LayerNumber)value;
        }
        public string name => "LayerNumber";
        public int priority => 0;
        public string fieldName => "LAYER: ";

        private int valueRunning => WorldGeneration.world?.biomeDepth ?? 0;
        private int valueStored = 0;
        
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
            return $"LAYER {s}";
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