using System.Collections.Generic;

namespace Converters.Monsters;

public class MonsterOutput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Race { get; set; }
    public int Experience { get; set; }
    public int Speed { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int ManaCost { get; set; }
    public int Corpse { get; set; }
    public LookData Outfit { get; set; } = new();
    public TargetChangeData TargetChange { get; set; }
    public StrategyData Strategy { get; set; }
    public Dictionary<string, int> Flags { get; set; } = new();
    public List<Attack> Attacks { get; set; }
    public DefenseData Defense { get; set; } = new();
    public List<Dictionary<string, object>> Defenses { get; set; }
    public Dictionary<string, int> Elements { get; set; }
    public List<LootData> Loot { get; set; }
    public VoicesData Voices { get; set; }
    public Dictionary<string, int> Immunities { get; internal set; }
    public SummonData Summon { get; set; }

    public class VoicesData
    {
        public int Interval { get; set; }

        public int Chance { get; set; }

        public List<Voice> Sentences { get; set; } = new();
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
        public string Name { get; internal set; }
        public int? Id { get; set; }
        public int Countmax { get; set; }
        public int Chance { get; set; }
        public List<LootData> Items { get; set; }
    }

    public class SummonData
    {
        public int MaxSummons { get; set; }
        public List<SummonCreatureData> Summons { get; set; } = new();
    }

    public class SummonCreatureData
    {
        public string Name { get; set; }
        public int Interval { get; set; }
        public int Chance { get; set; }
        public int Max { get; set; }
    }

    public class Attack
    {
        public string Name { get; set; }

        public int? Interval { get; set; }
        public int? Chance { get; set; }

        public int? MinDamage { get; set; }
        public int? MaxDamage { get; set; }
        public string DamageType { get; set; }

        public int? Range { get; set; }
        public int? Radius { get; set; }

        public string ShootEffect { get; set; }
        public string Effect { get; set; }

        public int? Length { get; set; }
        public int? Spread { get; set; }
        public bool? NeedTarget { get; set; }

        public AttackCondition Condition { get; set; }
        public Dictionary<string, object> Extra { get; set; }

        public class AttackCondition
        {
            public string Type { get; set; }
            public int Interval { get; set; }
            public int? TotalDamage { get; set; }
            public int? StartDamage { get; set; }
        }
    }
}