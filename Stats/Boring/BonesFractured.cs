using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class BonesFractured : StatGeneric<int> {
        public override string fieldName => "FRACTURES: ";

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.BreakBone))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            var instance = IStat.Get<BonesFractured>();
            if (!__instance.broken)
                instance.value += 1;
        }


        public override float Noteworthiness() {
            return 0;
        }
    }
}