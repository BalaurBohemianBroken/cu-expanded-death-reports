using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class LayerDepth : IStat {
        public string name => GetType().Name;
        public string fieldName => "DEPTH: ";

        private int valueRunning => WorldGeneration.world?.biomeDepth ?? 0;
        private int valueStored = 0;

        public float Noteworthiness() {
            return 0;
        }

        public string GetStatReadout(bool color) {
            // TODO: This doesn't seem to work. I might be better off just tracking this myself.
            var instance = IStat.Get<LayerDepth>();
            string s;
            if (this == instance)
                s = valueRunning.ToString();
            else
                s = valueStored.ToString();
            if (color)
                s = DeathReport.ColorVar(s);
            return s + " M";
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