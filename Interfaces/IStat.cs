using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace BalaurBohemianBroken {
    public interface IStat : ISerializable {
        public int priority { get; }
        public string name { get; }

        public bool IsNoteworthy();

        public void LoadToStatic();

        public string GetValue(int decimal_place = -1);
    }
}