using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Dislocations : StatGeneric<int> {
        public override string name => "Dislocations";
        public override string fieldName => "DISLOCATIONS: ";
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.BreakBone))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            var instance = IStat.Get<Dislocations>();
            if (!__instance.dislocated)
                instance.value += 1;
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}