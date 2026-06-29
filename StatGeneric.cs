using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

// TODO: Add character colours to notes.
namespace BalaurBohemianBroken {
    // These classes are a bit of a frankenstein of instance and static.
    // An instance is required to interact with the static stuff.
    // This is both a limitation of interfaces, and sort of a feature in that stats can be disabled.
    // Instances of the class can also be used just to store data, though, as is the case on the history screen.
    public abstract class StatGeneric<T> : IStat {
        protected static StatGeneric<T> instance;
        public virtual IStat runningInstance {
            get => instance;
            set => instance = (StatGeneric<T>)value;
        }
        public abstract string name { get; }
        public abstract int priority { get; }
        
        public T value { get; set; }

        public virtual string Serialize() {
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

        public virtual void Reset() {
            value = default;
        }

        public abstract bool IsNoteworthy();

        public string GetValue(int decimal_place = -1) {
            if (decimal_place > -1) {
                if (value is float vf)
                    return Math.Round(vf, decimal_place).ToString(CultureInfo.InvariantCulture);
            }
            return value.ToString();
        }
    }
}