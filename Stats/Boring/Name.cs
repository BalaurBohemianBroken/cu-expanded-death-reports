using HarmonyLib;
using System.Collections.Generic;

namespace BalaurBohemianBroken.Stats {
    [HarmonyPatch]
    public class Name : StatGeneric<string> {
        public override string name => "Name";
        public override string fieldName => "NAME: ";

        private static List<string> humanNames = new() {
            "Bosk",
            "Riesling",
            "Lilium",
            "Valeria",
            "Vale",
            "Balaur",
            "Lloyd",
            "Martin",
            "Merry",
            "Rey",
            "Cammy",
            "Tapp",
            "Eden",
            "Zelda",
            "Violet",
            "Bark",
            "Sanguine",
            "Rose",
            "Poppy",
            "Peanut",
            "Oscar",
            "Miles",
            "Lucky",
            "Loki",
            "Gusgus",
            "Gizmo",
            "Jade",
            "Evan",
            "Ginger",
            "Coco",
            "Brownie",
            "Cinnamon",
            "Chance",
            "Buddy",
            "Happy",
            "Future",
            "Comfort",
            "Sunset on park",
            "Twinkle",
            "Plush",
            "Kalia",
            "Pumpkin",
            "Ghost",
            "Citaa",
            "Arix",
            "Crystal",
            "Carter",
            "Carissa",
            "...",
            "Flint",
            "Ozzy",
            "Quant",
            "Solar",
            "Sigur",
            "Aran",
            "Wesker",
            "West",
            "Red",
            "Purple",
            "Blue",
            "Cloudy",
            "Skye",
            "Fox",
            "Milky",
            "Viz",
            "Light",
            "Riku",
            "Pika",
            "Penny",
            "Pearl",
            "sorry.",
            "Fluffy",
            "Innocence",
            "Ralsei",
            "Softie",
            "Cuddles",
            "Critter",
            "Yipper",
            "Yapper",
            "Beep",
            "Sparks",
            "Kyro",
            "Dani",
            "Bird",
        };

        // TODO: Some special notes that show the internal arguments over naming the creatures.
        public Name() {
            Reset();
        }

        public override string GetStatReadout(bool color) {
            string s = value;
            if (color)
                s = DeathReport.ColorS(s);
            return s;
        }
        
        public sealed override void Reset() {
            // TODO: Check previous runs to ensure unique name.
            value = humanNames.PickRandom();
        }

        public override float Noteworthiness() {
            return 0;
        }
    }
}