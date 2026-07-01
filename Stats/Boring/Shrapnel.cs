using System.Collections.Generic;
using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Shrapnel : StatGeneric<int> {
        public override string fieldName => "SHRAPNEL: ";

        private Dictionary<Limb, int> shrapnelLastFrame = new();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            var instance = IStat.Get<Shrapnel>();
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            int last_shrapnel = instance.shrapnelLastFrame.GetValueOrDefault(__instance, 0);
            if (__instance.shrapnel > last_shrapnel) {
                instance.value += __instance.shrapnel - last_shrapnel;
            }
            instance.shrapnelLastFrame[__instance] = last_shrapnel;
        }

        public override void Reset() {
            value = 0;
            shrapnelLastFrame = new Dictionary<Limb, int>();
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}