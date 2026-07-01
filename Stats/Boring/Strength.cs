using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Strength : IStat {
        private static Strength instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (Strength)value;
        }
        public string name => "Strength";
        public int priority => 0;
        public string fieldName => "STRENGTH: ";

        private int valueRunning => PlayerCamera.main?.body?.skills?.STR ?? 0;
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
            return s + " STR";
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