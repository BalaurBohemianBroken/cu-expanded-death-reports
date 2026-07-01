using System.Collections.Generic;
using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class CutsOpened : StatGeneric<int> {
        public override string name => "CutsOpened";
        public override string fieldName => "CUTS OPENED: ";
        
        private Dictionary<Limb, bool> wasCutLastFrame = new();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            // TODO: Serialize this dictionary.
            var instance = IStat.Get<CutsOpened>();
            bool is_cut = __instance.bleedAmount > 0;
            if (is_cut && !instance.wasCutLastFrame.GetValueOrDefault(__instance, true))
                instance.value += 1;
            instance.wasCutLastFrame[__instance] = __instance.bleedAmount > 0;
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}