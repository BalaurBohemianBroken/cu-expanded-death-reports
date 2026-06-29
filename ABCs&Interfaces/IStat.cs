using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace BalaurBohemianBroken {
    public interface IStat : ISerializable {
        // Should assign to a static variable that can be accessed from patches.
        // This is the version of the IStat that is currently tracking data.
        // I'd prefer this to be an abstract static to remove the middleman,
        // but game's runtime doesn't allow it I don't think.
        public IStat runningInstance { get; set; }
        public int priority { get; }
        public string name { get; }
        // TODO: What this field should be identified as on the report.
        // public string fieldName { get; }

        public float Noteworthiness();

        public string GetStatReadout();

        public void Reset();
    }
}