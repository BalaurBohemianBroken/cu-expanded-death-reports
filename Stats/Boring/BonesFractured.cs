using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class BonesFractured : StatGeneric<int> {
        public override string name => "BonesFractured";
        public override int priority => 0;
        public override string fieldName => "FRACTURES: ";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.BreakBone))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            if (!__instance.broken)
                instance.value += 1;
        }


        public override float Noteworthiness() {
            return 0;
        }
    }
}