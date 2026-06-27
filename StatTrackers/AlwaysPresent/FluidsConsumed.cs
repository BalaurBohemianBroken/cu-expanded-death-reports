using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class FluidsConsumedStat : StatGeneric<float> {
        public override string name => "FluidsConsumed";
        public override int priority => 0;
        
        private List<string> _notes = new List<string>() {
        };
        protected override List<string> notes => _notes;
        
        // Because I can't just patch onDrink, as it is a delegate method, I try to intercept it at an earlier point.
        // Not perfect, but faster to write.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaterContainerItem), nameof(WaterContainerItem.Drink))]
        public static void PatchDrinkFromContainer(float amount, WaterContainerItem __instance) {
            value_running += Mathf.Min(amount, __instance.CurrentTotal);
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
            if (liquid_id > 0 && liquid_id < 6)
                value_running += 200;
        }

        public override bool IsNoteworthy() {
            return false;
        }
    }
}