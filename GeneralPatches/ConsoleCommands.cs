using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace BalaurBohemianBroken {
    [HarmonyPatch]
    public class ConsoleCommands {
        private static ConsoleScript instance;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ConsoleScript), nameof(ConsoleScript.RegisterAllCommands))]
        private static void Postfix(ConsoleScript __instance) {
            instance = __instance;
            Command c;
            c = new Command("BalaurStatPrint", "Echoes currently running value_runnings.", PrintStats,
                new Dictionary<int, List<string>> {
                    {0, ExpandedDeathReports.StatTrackerNames}
                },
                ("Stat name", "Internal name of the stat to print.")
            );
            ConsoleScript.Commands.Add(c);
            c = new Command("BalaurStatAll", "Echoes a list of all trackers currently enabled.", PrintAllTrackers, null);
            ConsoleScript.Commands.Add(c);
        }

        private static void PrintStats(string[] args) {
            if (args.Length < 2) {
                return;
            }

            for (int i = 1; i < args.Length; i++) {
                if (!ExpandedDeathReports.StatTrackersAll.TryGetValue(args[i], out IStat tracker)) {
                    instance.LogToConsole($"Cannot find tracker named {args[i]}.");
                    return;
                }

                instance.LogToConsole($"{args[i]}: {tracker.Serialize()} ({tracker.GetStatReadout()})");
            }
        }
        
        private static void PrintAllTrackers(string[] args) {
            instance.LogToConsole($"==== Current stats ====");
            foreach (KeyValuePair<string, IStat> kvp in ExpandedDeathReports.StatTrackersAll) {
                instance.LogToConsole($"{kvp.Value.name}: {kvp.Value.Serialize()} ({kvp.Value.GetStatReadout()})");
            }
        }
    }
}