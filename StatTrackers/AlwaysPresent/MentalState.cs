using System;
using HarmonyLib;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BalaurBohemianBroken.StatTrackers {
    [HarmonyPatch]
    public class MentalState : IStat {
        public string name => "MentalState";
        public string fieldName => "MENTALS: ";
        private static MentalState instance;
        public IStat runningInstance {
            get => instance;
            set => instance = (MentalState)value;
        }
        public int priority => 0;
        
        private static int stateRunning {
            get {
                Body b = PlayerCamera.main?.body;
                if (b != null)
                    return Mathf.RoundToInt(b.AverageHappiness() * 0.1f);
                return 0;
            }
        }
        private static bool wipedRunning {
            get {
                Body b = PlayerCamera.main?.body;
                if (b != null)
                    return b.mindWipe;
                return false;
            }
        }
        private static bool lastStandRunning {
            get {
                Body b = PlayerCamera.main?.body;
                if (b != null)
                    return b.succesfullyRolledLastStand;
                return false;
            }
        }

        private int state;
        private bool lastStand;
        private bool wiped;

        public string GetStatReadout() {
            string mood = "moodrange";
            if (wiped) 
                mood += "wiped";
            else
                mood += state;
            if (lastStand)
                mood += Locale.GetOther("moodrangeyethopeful");
            return Locale.GetOther(mood).ToUpper();
        }

        public void Reset() {
            state = 0;
            lastStand = false;
            wiped = false;
        }
        public float Noteworthiness() {
            // TODO: Comment on mind wipe, last stand.
            return 0;
        }

        public virtual string Serialize() {
            try {
                Dictionary<string, string> data = new Dictionary<string, string>() {
                    {"state", JsonConvert.SerializeObject(stateRunning)},
                    {"wiped", JsonConvert.SerializeObject(wipedRunning)},
                    {"lastStand", JsonConvert.SerializeObject(lastStandRunning)},
                };
                return JsonConvert.SerializeObject(data);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to serialize object.\nName: {name}\nValues: {stateRunning},  {lastStand}, {wiped}");
                throw;
            }
        }
        
        public void Deserialize(string serialized) {
            try {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized);
                state = JsonConvert.DeserializeObject<int>(data["state"]);
                wiped = JsonConvert.DeserializeObject<bool>(data["wiped"]);
                lastStand = JsonConvert.DeserializeObject<bool>(data["lastStand"]);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
    }
}