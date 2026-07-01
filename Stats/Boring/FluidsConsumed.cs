using System;
using System.Globalization;
using HarmonyLib;
using UnityEngine;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class FluidsConsumed : StatGeneric<float> {
        public override string name => "FluidsConsumed";
        public override string fieldName => "FLUIDS: ";

        // Because I can't just patch onDrink, as it is a delegate method, I try to intercept it at an earlier point.
        // Not perfect, but faster to write.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaterContainerItem), nameof(WaterContainerItem.Drink))]
        public static void PatchDrinkFromContainer(float amount, WaterContainerItem __instance) {
            var instance = IStat.Get<FluidsConsumed>();
            instance.value += Mathf.Min(amount, __instance.CurrentTotal);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FluidManager), nameof(FluidManager.DrinkLiquid))]
        public static void PatchDrinkFromFloor(Vector2Int pos, Body body, FluidManager __instance) {
            if (!ExpandedDeathReports.IsMainBody(body))
                return;
            var instance = IStat.Get<FluidsConsumed>();
            int liquid_id = __instance.GetLiquid(pos.x, pos.y);
            bool has_filter_straw = body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).id == "filterstraw";
            if (liquid_id == 3 && has_filter_straw)
                return;
            if (liquid_id is > 0 and < 6)
                instance.value += 200;
        }

        public override string GetStatReadout(bool color) {
            string num = Math.Round(value, 0).ToString(CultureInfo.InvariantCulture);
            if (color)
                num = DeathReport.ColorVar(num);
            return num + " ML";
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}