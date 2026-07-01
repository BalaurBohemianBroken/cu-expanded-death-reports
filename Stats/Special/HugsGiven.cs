using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BalaurBohemianBroken.Stats;

[HarmonyPatch]
public class HugsGiven : StatGeneric<int> {
    public override string fieldName => "HUGS: ";

    private List<string> notes = new() {
        DeathReport.ColorFinsky("Similar in labs. Must be instinctual."),
        DeathReport.ColorFinsky("An idea for the plush line."),
        DeathReport.ColorHal("Pointless."),
        DeathReport.ColorS("At least that's nice."),
        DeathReport.ColorS("Cute..."),
    };

    private bool hostile_from_hug = false;
    private List<string> hostile_notes = new() {
        DeathReport.ColorS("One of the creatures turned aggressive from an attempted hug. It's tragic what stress can drive them to do."),
        DeathReport.ColorHal("This one fumbled social norms, and turned another experiment hostile in an attempt to hug it. Did it think it was worth it?"),
        DeathReport.ColorFinsky("A failed hug became a hostile encounter."),
    };

    // TODO: Maybe use this for something.
    public int hugs_failed = 0;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TraderScript), nameof(TraderScript.TryHug))]
    public static void Patch(TraderScript __instance) {
        var instance = IStat.Get<HugsGiven>();
        if (__instance.reputation < __instance.minHugReputation || __instance.body.dirtyness > 50f) {
            instance.hugs_failed += 1;
            return;
        }

        // I had a weird instance where this number was going up once per frame. I can't replicate it.
        // I had a similar thing happen with bullets, once. No clue why.
        // if (!__instance.didHug) {
        instance.value += 1;
        // }

        if (__instance.reputation < 30f) {
            instance.hugs_failed += 1;
            instance.hostile_from_hug = true;
        }
    }

    public override float Noteworthiness() {
        float val = 0;
        // 1 point per 5 hugs, up to 3.
        val += Mathf.Min(value / 5, 3);
        if (hostile_from_hug)
            val += 3;
        return val;
    }

    public override void Reset() {
        value = 0;
        hugs_failed = 0;
        hostile_from_hug = false;
    }
}