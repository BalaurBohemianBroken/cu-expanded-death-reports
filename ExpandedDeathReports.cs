using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BalaurBohemianBroken.Stats;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;

// TODO: Option to remove old reports, "burn" them
// TODO: Post-it notes for the 'special' report features.
// TODO: Add support for multiple save locations. See how Jimmy modifies it.
// TODO: Cause of death?

// TODO: Saving and loading of reports.
// TODO: Print a second report if there are extended notes. Move first report to middle of screen on receipt rip.
// TODO: Add more stats.
// TODO: Menu page for viewing old runs.
namespace BalaurBohemianBroken {
    [BepInPlugin("yip.balaur.ExpandedDeathReports", "ExpandedDeathReports", "1.0.0")]
    public class ExpandedDeathReports : BaseUnityPlugin {
        public static ManualLogSource logger;
        // Based on the game's SaveSystem.SaveGame function.
        public static string savePathStats = Application.persistentDataPath + "\\death_reports.yip";
        public static string savePathOngoing = Application.persistentDataPath + "\\ongoing_run.yip";

        // public static Dictionary<string, IStat> StatTrackersBoring { get; private set; } = new Dictionary<string, IStat>();
        // public static Dictionary<string, IStat> StatTrackersSpecial { get; private set; } = new Dictionary<string, IStat>();
        public static Dictionary<string, IStat> StatTrackersAll { get; private set; } = new Dictionary<string, IStat>();
        public static List<string> StatTrackerNames { get; private set; } = new List<string>();

        public void Awake() {
            logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            FillStatTrackers();
        }

        // TODO: Regenerate static values on game start.
        public static void FillStatTrackers() {
            StatTrackerNames = new List<string>();
            StatTrackersAll = new Dictionary<string, IStat>();
            
            // Based on:
            // https://stackoverflow.com/questions/5120647/instantiate-all-classes-implementing-a-specific-interface
            var interfaceType = typeof(IStat);
            var all = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x));

            foreach (IStat stat in all) {
                if (StatTrackersAll.ContainsKey(stat.name))
                    logger.LogWarning($"Duplicate of stat tracker with name {stat.name}");
                StatTrackerNames.Add(stat.name);
                StatTrackersAll[stat.name] = stat;

                stat.runningInstance = stat;
            }
        }

        // Save the list of currently running IStats out to a dictionary
        public void SaveRunning() {
            Dictionary<string, string> stat_data = new Dictionary<string, string>();
            foreach (KeyValuePair<string, IStat> kvp in StatTrackersAll) {
                stat_data[kvp.Key] = kvp.Value.Serialize();
            }
            string save_data = JsonConvert.SerializeObject(stat_data, Formatting.None, new JsonSerializerSettings());
            File.WriteAllBytes(savePathOngoing, SaveSystem.Zip(save_data));
        }

        public void LoadRunning() {
            if (!File.Exists(savePathOngoing)) {
                return;
            }

            string raw_data = SaveSystem.Unzip(File.ReadAllBytes(savePathOngoing));
            Dictionary<string, string> saved_data = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw_data);
            
            foreach (KeyValuePair<string, IStat> kvp in StatTrackersAll) {
                // TODO: Some kind of logging here for when a key isn't found. Probably just warning.
                if (saved_data.TryGetValue(kvp.Key, out string object_data)) {
                    kvp.Value.Deserialize(object_data);
                }
            }
        }

        public static bool IsMainBody(Body body) {
            // This is placed for multiplayer compatibility. Maybe. Idk how multiplayer works right now, just assuming.
            return PlayerCamera.main.body == body;
        }

        // Taken from: https://discussions.unity.com/t/random-numbers-with-a-weighted-chance/646590/3
        public static int GetRandomWeightedIndex(List<float> weights, float total_weight = -1) {
            if (weights == null || weights.Count == 0)
                throw new ArgumentException();

            if (Mathf.Approximately(total_weight, -1)) {
                foreach (float weight in weights) {
                    if (weight < 0 || float.IsNaN(weight))
                        throw new ArgumentException();
                    total_weight += weight;
                }
            }

            float random_number = UnityEngine.Random.value;
            float sum = 0f;
            for (int i = 0; i < weights.Count; i++) {
                float weight = weights[i];
                sum += weight / total_weight;
                if (sum >= random_number)
                    return i;
            }
  
            throw new ArithmeticException();
        }
    }
}