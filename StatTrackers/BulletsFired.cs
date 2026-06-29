using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers;

[HarmonyPatch]
public class BulletsFired : StatGeneric<int> {
    public override string name => "BulletsFired";
    public override int priority => 0;

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