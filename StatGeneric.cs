using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace BalaurBohemianBroken {
    // These classes are a bit of a frankenstein of instance and static.
    // An instance is required to interact with the static stuff.
    // This is both a limitation of interfaces, and sort of a feature in that stats can be disabled.
    // Instances of the class can also be used just to store data, though, as is the case on the history screen.
    public abstract class StatGeneric<T> : IStat {
        public abstract string name { get; }
        public abstract int priority { get; }
        
        protected static T value_running { get; set; } // A value that increments during runs.
        public T value { get; protected set; }

        protected abstract List<string> notes { get; }
        
        public virtual string Serialize() {
            LoadFromStatic();
            try {
                return JsonConvert.SerializeObject(value);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to serialize object.\nName: {name}\nValue: {value}");
                throw;
            }
        }
        
        public virtual void Deserialize(string serialized) {
            try {
                value = JsonConvert.DeserializeObject<T>(serialized);
            }
            catch (JsonException) {
                ExpandedDeathReports.logger.LogWarning($"Failed to deserialize object.\nName: {name}\nSerialization string: {serialized}");
                throw;
            }
        }
        
        protected virtual void LoadFromStatic() {
            value = value_running;
        }

        public virtual void LoadToStatic() {
            value_running = value;
        }

        public abstract bool IsNoteworthy();

        public static void Reset() {
            // I'm not defaulting value because I currently use this only to store previous runs. No reason to default it.
            value_running = default;
        }
    }
}