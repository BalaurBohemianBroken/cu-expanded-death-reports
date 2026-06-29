using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class CaloriesConsumed : IStat {
        public string name => "CaloriesConsumed";
        public IStat runningInstance { get; set; }
        public int priority => 0;

        private int valueRunning => PlayerCamera.main?.caloriesConsumed ?? 0;
        private int value;

        public string GetStatReadout(int decimal_place = -1) {
            // TODO: This is not a perfect solution, but it works for now. Check when codebase is more finished that this still works.
            if (value == 0)
                value = valueRunning;
            return $"{value} KCAL";
        }

        public void Reset() {
            value = 0;
        }

        public bool IsNoteworthy() {
            return false;
        }

        public string Serialize() {
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
                value = JsonConvert.DeserializeObject<int>(serialized);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}