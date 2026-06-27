using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class BulletsFired : StatGeneric<int> {
        public override string name => "BulletsFired";
        public override int priority => 0;

        private int noteworthy_threshold = 30;
        private List<string> _notes = new List<string>() {
            "Did we train them on this? No."
        };
        protected override List<string> notes => _notes;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GunScript), nameof(GunScript.Fire))]
        public static void Patch() {
            value_running += 1;
        }

        public override bool IsNoteworthy() {
            return value > noteworthy_threshold;
        }
    }
}