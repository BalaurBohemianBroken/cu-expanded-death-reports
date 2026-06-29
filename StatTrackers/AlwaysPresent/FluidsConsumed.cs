using System;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;
using UnityEngine;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class FluidsConsumed : StatGeneric<float> {
        public override string name => "FluidsConsumed";
        public override int priority => 0;
        
        // Because I can't just patch onDrink, as it is a delegate method, I try to intercept it at an earlier point.
        // Not perfect, but faster to write.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaterContainerItem), nameof(WaterContainerItem.Drink))]
        public static void PatchDrinkFromContainer(float amount, WaterContainerItem __instance) {
            instance.value += Mathf.Min(amount, __instance.CurrentTotal);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FluidManager), nameof(FluidManager.DrinkLiquid))]
        public static void PatchDrinkFromFloor(Vector2Int pos, Body body, FluidManager __instance) {
            if (!ExpandedDeathReports.IsMainBody(body))
                return;
            int liquid_id = __instance.GetLiquid(pos.x, pos.y);
            bool has_filter_straw = body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).id == "filterstraw";
            if (liquid_id == 3 && has_filter_straw)
                return;
            if (liquid_id is > 0 and < 6)
                instance.value += 200;
        }

        public override string GetStatReadout(int decimal_place = -1) {
            string num;
            if (decimal_place > -1)
                num = Math.Round(value, decimal_place).ToString(CultureInfo.InvariantCulture);
            else
                num = value.ToString(CultureInfo.InvariantCulture);
            return num + " ML";
        }

        public override bool IsNoteworthy() {
            return false;
        }
    }
}