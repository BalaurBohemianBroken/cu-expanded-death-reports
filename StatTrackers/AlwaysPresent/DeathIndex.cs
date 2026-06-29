using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace BalaurBohemianBroken.StatTrackers {
    // This gets filled by the EndScreenPatch
    public class DeathIndex : StatGeneric<int> {
        public override string name => "DeathIndex";
        public override int priority => 0;
        
        public override float Noteworthiness() {
            // TODO: Milestones?
            return 0;
        }
    }
}