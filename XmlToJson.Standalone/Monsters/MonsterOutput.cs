using System.Collections.Generic;

namespace XmlToJson.Standalone.Monsters
{
    public class MonsterOutput
    {
        public string Name { get; set; }
        public string NameDescription { get; set; }
        public string Race { get; set; }
        public int Experience { get; set; }
        public int Speed { get; set; }
        public int ManaCost { get; set; }
        public HealthData Health { get; set; } = new HealthData();
        public LookData Look { get; set; } = new LookData();
        public TargetChangeData TargetChange { get; set; } = new TargetChangeData();
        public StrategyData Strategy { get; set; } = new StrategyData();
        public Dictionary<string, int> Flags { get; set; } = new Dictionary<string, int>();
        public List<Dictionary<string, object>> Attacks { get; set; } = new List<Dictionary<string, object>>();
        public DefenseData Defense { get; set; } = new DefenseData();
        public List<Dictionary<string, object>> Defenses { get; set; } = new List<Dictionary<string, object>>();
        public Dictionary<string, int> Elements { get; set; } = new Dictionary<string, int>();
        public List<LootData> Loot { get; set; } = new List<LootData>();
        public VoicesData Voices { get; set; } = new VoicesData();
        public Dictionary<string, int> Immunities { get; set; } = new Dictionary<string, int>();
        public SummonData Summon { get; set; } = new SummonData();

        public class VoicesData
        {
            public int Interval { get; set; }

            public int Chance { get; set; }

            public List<Voice> Sentences { get; set; } = new List<Voice>();
        }

        public class Voice
        {
            public string Sentence { get; set; }
            public bool Yell { get; internal set; }
        }

        public class HealthData
        {
            public int Max { get; set; }
            public int Now { get; set; }
        }

        public class LookData
        {
            public int Type { get; set; }
            public int Head { get; set; }
            public int Body { get; set; }
            public int Legs { get; set; }
            public int Feet { get; set; }
            public int Corpse { get; set; }
        }

        public class TargetChangeData
        {
            public int Interval { get; set; }
            public int Chance { get; set; }
        }

        public class StrategyData
        {
            public int Attack { get; set; }
            public int Defense { get; set; }
        }

        public class DefenseData
        {
            public int Armor { get; set; }
            public int Defense { get; set; }
        }

        public class LootData
        {
            public int Id { get; set; }

            public int Countmax { get; set; }

            public int Chance { get; set; }

            public List<LootData> Items { get; set; } = new List<LootData>();
            public string Name { get; internal set; }
        }

        public class SummonData
        {
            public int MaxSummons { get; set; }
            public List<SummonCreatureData> Summons { get; set; } = new List<SummonCreatureData>();
        }

        public class SummonCreatureData
        {
            public string Name { get; set; }
            public int Interval { get; set; }
            public int Chance { get; set; }
            public int Max { get; set; }
        }
    }
}