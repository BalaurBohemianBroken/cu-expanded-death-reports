using HarmonyLib;
using System.Collections.Generic;

namespace BalaurBohemianBroken.Stats;

[HarmonyPatch]
public class BulletsFired : StatGeneric<int> {
    public override string name => "BulletsFired";
    public override int priority => 0;
    public override string fieldName => "SHOTS FIRED: ";

    private int noteworthy_threshold = 30;
    private List<string> _notes = new() {
        "Did we train them on this? No."
    };

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GunScript), nameof(GunScript.Fire))]
    public static void Patch() {
        instance.value += 1;
    }

    public override float Noteworthiness() {
        // TODO: Scale this with depth.
        if (value >= noteworthy_threshold)
            return 1;
        return 0;
    }
}