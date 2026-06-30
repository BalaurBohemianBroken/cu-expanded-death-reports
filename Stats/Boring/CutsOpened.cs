using System.Collections.Generic;
using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class CutsOpened : StatGeneric<int> {
        public override string name => "CutsOpened";
        public override int priority => 0;
        public override string fieldName => "CUTS OPENED: ";
        
        private static Dictionary<Limb, bool> wasCutLastFrame = new();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            // Because I'm not clearing this value, theoretically cuts could be carried over between runs.
            // But I really can't see it.
            bool is_cut = __instance.bleedAmount > 0;
            if (is_cut && !wasCutLastFrame.GetValueOrDefault(__instance, true))
                instance.value += 1;
            wasCutLastFrame[__instance] = __instance.bleedAmount > 0;
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}