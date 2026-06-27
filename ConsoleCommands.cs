using BepInEx;
using HarmonyLib;

namespace BalaurBohemianBroken {
    [HarmonyPatch]
    public class ConsoleCommands {
        public static ConsoleScript instance;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ConsoleScript), nameof(ConsoleScript.RegisterAllCommands))]
        public static void Postfix(ConsoleScript __instance) {
            instance = __instance;
            Command c = new Command("BalaurPrintStat", "Echoes a currently running value_running", PrintStat, null);
            ConsoleScript.Commands.Add(c);
        }

        public static void PrintStat(string[] args) {
            if (args.Length < 2) {
                return;
            }

            if (!ExpandedDeathReports.stat_trackers_all.TryGetValue(args[1], out IStat tracker)) {
                instance.LogToConsole($"Cannot find tracker named {args[1]}.");
                return;
            }

            instance.LogToConsole($"{args[1]}: {tracker.Serialize()}");
        }
    }
}