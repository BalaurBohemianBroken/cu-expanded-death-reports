using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BalaurBohemianBroken.StatTrackers;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;

// TODO: Option to remove old reports, "burn" them
// TODO: Post-it notes for the 'special' report features.
// TODO: Add support for multiple save locations. See how Jimmy modifies it.
namespace BalaurBohemianBroken {
    [BepInPlugin("yip.balaur.ExpandedDeathReports", "ExpandedDeathReports", "1.0.0")]
    public class ExpandedDeathReports : BaseUnityPlugin {
        public static ManualLogSource logger;
        // Based on the game's SaveSystem.SaveGame function.
        public static string save_path_stats = Application.persistentDataPath + "\\death_reports.yip";
        public static string save_path_ongoing = Application.persistentDataPath + "\\ongoing_run.yip";

        public static Dictionary<string, IStat> stat_trackers = new Dictionary<string, IStat>();
        
        public void Awake() {
            logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            FillStatTrackers();
        }

        public static void FillStatTrackers() {
            List<IStat> main_stat_tracker_list = new List<IStat>() {
                new BonesFractured(),
                new Dislocations(),
                new FluidsConsumedStat(),
                new Infections(),
                new PainSufferedAverage(),
            };
            List<IStat> special_stat_tracker_list = new List<IStat>() {
                new Amputations(),
                new BulletsFired(),
                new PainSufferedTotal()
            };

            stat_trackers = new Dictionary<string, IStat>();
            foreach (IStat stat in main_stat_tracker_list) {
                stat_trackers[stat.name] = stat;
            }
        }

        // Save the list of currently running IStats out to a dictionary
        public void SaveRunning() {
            Dictionary<string, string> stat_data = new Dictionary<string, string>();
            foreach (KeyValuePair<string, IStat> kvp in stat_trackers) {
                stat_data[kvp.Key] = kvp.Value.Serialize();
            }
            string save_data = JsonConvert.SerializeObject(stat_data, Formatting.None, new JsonSerializerSettings());
            File.WriteAllBytes(save_path_ongoing, SaveSystem.Zip(save_data));
        }

        public void LoadRunning() {
            if (!File.Exists(save_path_ongoing)) {
                return;
            }

            string raw_data = SaveSystem.Unzip(File.ReadAllBytes(save_path_ongoing));
            Dictionary<string, string> saved_data = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw_data);
            
            foreach (KeyValuePair<string, IStat> kvp in stat_trackers) {
                // TODO: Some kind of logging here for when a key isn't found. Probably just warning.
                if (saved_data.TryGetValue(kvp.Key, out string object_data)) {
                    kvp.Value.Deserialize(object_data);
                    kvp.Value.LoadToStatic();
                }
            }
        }

        public static bool IsMainBody(Body body) {
            // This is placed for multiplayer compatibility. Maybe. Idk how multiplayer works right now, just assuming.
            return PlayerCamera.main.body == body;
        }
    }

    public class DeathReport {
        // Report layout
        // MSN-CARGO DETAILED STATUS REPORT #run_num
        // date
        // UNRESOLVED
        //
        // specimen_id   name
        // MENTALS: mental_state
        // RESULTS: depth M, LAYER layer, timespan  
        // 
        // CONSUMPTION: calories KCAL, fluids_consumed ML
        // QUALITIES: int INT, str STR, res RES
        // 
        // PAIN UNITS: pain_suffered
        // FRACTURES/BREAKS: bones_fractured/bones_broken
        // CUTS/SHRAPNEL/INFECTIONS: opened_cuts/shrapnel/infections
        // DAMAGE RECEIVED/RECOVERED: injuries_received/injuries_recovered
        // 
        // FURTHER NOTES
        // 
        //
        // 
        // 
        //
        // 
        // 
        
        // Special fields:
        // THINGS CREATED: created
        // UNIQUE THINGS CREATED: created_unique
        // DISFIGURATIONS: disfigurations
        // DRUGS CONSUMED: drugs_consumed
        // TIME SLEPT: time_slept
        // EXPERIMENTS MET: experiments_met
        // TRADES: trades_made
          // An aptitude for material gain.
          // name a trader that they maxed the trading with.
        // HUGS GIVEN: hugs_given
          // This is undesired.
          // Why are they so keen on this?
          // Cute.
          // Some respite, at least.
          // Could we include this in conditioning? No.
        // FENTANYL CONSUMED: fent_consumed
          // This may explain recent inventory discrepancies. Medical security will be raised.
          // But when, and why?
        // SHOTS FIRED: bullets_fired
          // I don't recall this being part of their briefing.
        // KILLS: creatures_killed
        // DESTRUCTION: things_destroyed
        
        // Special behaviours:
        // Operator received a phone call. Experiment ended prematurely.
        // Operator decided experiment was a __failure__. Experiment ended prematurely.
        // Operator decided to end shift early. Experiment ended prematurely.
        // Operator became distressed. Dismissed. Experiment ended prematurely.
        // Operator displayed lack of interest in experiment. Suggested ending experiment prematurely. Accepted.
        // Operator had romantic encounter to attend. Experiment ended prematurely.
        // Operator fell asleep. Experiment ended prematurely.
        // Operator was found naming experiment. Involved individuals have a meeting at shift start tomorrow. Experiment ended prematurely.
        // Operator expressed views contradictory to objective. Required sedation and removal. Experiment ended prematurely.
        // Operator became distracted. Experiment ended prematurely.
        
        // Subject spent significant time complaining about equipment load, yet did nothing to solve this. Is this kleptomanical trend genetic or environmental?
        // Subject was slow. Consistently needed negative reinforcement to proceed. Possibilities: Forgetful. Lazy. Undesirably cautious. Explicitly disregarding objective. Suggest more severe briefing routine.
        // Subject failed to comply with negative reinforcement. We cannot waste resources monitoring ineffective experiments. S., this is your job, please address.
        // Subject progressed with significant efficiency, and significant results. May be anomalous data point. Suggest corroborating results with other departments to identify correlation. 
        // Subject progressed with significant efficiency, yet showed insufficient results. S., both extremes of fear conditioning are statistically ineffective.
        // Subject consumed a light bulb. We have agreed to accept this as an accident. A repeat would be most concerning.
        // A second subject has consumed a light bulb. We may need to analyse their generic palette. Possibly mutated.
        // A third subject has consumed a light bulb. Tests have shown they do not prefer the taste of glass or bulb related materials to dog food. Further study needed.
        // A fourth subject has consumed a light bulb. Recommendations were suggested to teach them to place the small part in their mouth instead. However, we are not wasting resources on this.
        // Subjects continue to consume light bulbs at an alarming rate. This goes against basic survival instinct and calls into question our neural sequencing. Recommended we do not record this further.
        
        // Things to fit in:
        // longest distance fallen with no damage
        // longest distance with damage
        // traps destroyed
        // brain damag
        // bandages used
        // chest drains used
        // ores mined
        // 
        // suffered stroke
        // survived stroke
        // restarted heart
        // triggered multiple landmines
        // shot multiple times
        // killed salad
        // killed traders
        // eaten by dune
        // made exclusive recipes
        // listened to music
        // very long run
        // epdas read
        // suffered significant disfiguration
        // 
        // prosthetics added
        public int run_num;
        public int depth;
        public int calories;
        
        public string timespan;  // Useless.
        public string date;
        
        public string specimen_id;
        public string mental_state;
        
        // Custom fields
        public string name;  // Maybe make this drawn on in pen or something?
        public float fluids_consumed;
        public int layer;

        public int intelligence;  // Such a waste.
        public int strength;
        public int resilience;
        
        public int pain_suffered;  // Poor thing...

        public int dislocations;
        public int bones_broken;
        
        public int opened_cuts;
        public int infections;
        public float injuries_recovered;
        public float drugs_taken;  // Predisposed.
        
        public int things_said;  // Chatty.
        
        public int experiments_met;

        public int trades_made;

        public int things_created;
        public int unique_things_created;  // How'd they figure this out?
        
        public float time_slept;  // Why is this batch so lazy?
        
        public int bullets_fired;  // Did we train them on this?
        
        public int creatures_killed;  // This one snapped.
        
        public int things_destroyed;
        
        // stats i haven't got a place for yet
        // maybe a more expansive stats page for the whole game would be better for stuff like this?
        public float fent_consumed;
    }
}