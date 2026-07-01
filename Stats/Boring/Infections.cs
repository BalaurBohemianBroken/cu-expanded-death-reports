using System.Collections.Generic;
using HarmonyLib;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Infections : StatGeneric<int> {
        public override string fieldName => "INFECTIONS: ";
        private Dictionary<Limb, bool> infectionLastCheck = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Limb), nameof(Limb.Update))]
        public static void PatchUpdate(Limb __instance) {
            if (!ExpandedDeathReports.IsMainBody(__instance.body))
                return;
            // TODO: Serialize this data.
            var instance = IStat.Get<Infections>();
            if (instance.infectionLastCheck.GetValueOrDefault(__instance, false) && __instance.infected)
                instance.value += 1;
            instance.infectionLastCheck[__instance] = __instance.infected;
        }

        public override void Reset() {
            value = 0;
            infectionLastCheck = new Dictionary<Limb, bool>();
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}