using HarmonyLib;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.Stats;

[HarmonyPatch]
public class Age : IStat {
    public string name => GetType().Name;
    public string fieldName => "AGE: ";

    private int valueRunning => WoundView.view.cInfo[1];
    private int value;

    public string GetStatReadout(bool color) {
        // TODO: This is not a perfect solution, but it works for now. Check when codebase is more finished that this still works.
        if (value == 0)
            value = valueRunning;
        string s = value.ToString();
        if (color)
            s = DeathReport.ColorVar(s);
        return s;
    }

    public void Reset() {
        value = 0;
    }

    public float Noteworthiness() {
        return 0;
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