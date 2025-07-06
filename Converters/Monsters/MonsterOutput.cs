using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Converters.Monsters;

public class MonsterOutput
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("nameDescription")] public string NameDescription { get; set; }

    [JsonPropertyName("race")] public string Race { get; set; }

    [JsonPropertyName("experience")] public uint Experience { get; set; }

    [JsonPropertyName("speed")] public ushort Speed { get; set; }

    [JsonPropertyName("manacost")] public ushort ManaCost { get; set; }

    [JsonPropertyName("health")] public HealthData Health { get; set; } = new HealthData();

    [JsonPropertyName("look")] public LookData Look { get; set; } = new LookData();

    [JsonPropertyName("targetchange")] public TargetchangeData TargetChange { get; set; } = new TargetchangeData();

    [JsonPropertyName("strategy")] public StrategyData Strategy { get; set; } = new StrategyData();

    [JsonPropertyName("flags")] public IDictionary<string, ushort> Flags { get; set; } = new Dictionary<string, ushort>();

    [JsonPropertyName("attacks")] public List<Dictionary<string, object>> Attacks { get; set; } = new();

    [JsonPropertyName("defenses")] public List<Dictionary<string, object>> Defenses { get; set; } = new();

    [JsonPropertyName("defense")] public DefenseData Defense { get; set; } = new DefenseData();

    [JsonPropertyName("elements")] public Dictionary<string, sbyte> Elements { get; set; } = new Dictionary<string, sbyte>();

    [JsonPropertyName("immunities")] public Dictionary<string, byte> Immunities { get; set; } = new Dictionary<string, byte>();

    [JsonPropertyName("voices")] public VoicesData Voices { get; set; } = new VoicesData();

    [JsonPropertyName("summon")] public SummonData Summon { get; set; } = new SummonData();

    [JsonPropertyName("loot")] public List<LootData> Loot { get; set; } = new List<LootData>();

    public class HealthData
    {
        [JsonPropertyName("now")] public uint Now { get; set; }

        [JsonPropertyName("max")] public uint Max { get; set; }
    }

    public class LookData
    {
        [JsonPropertyName("type")] public ushort Type { get; set; }

        [JsonPropertyName("corpse")] public ushort Corpse { get; set; }

        [JsonPropertyName("body")] public ushort Body { get; set; }

        [JsonPropertyName("legs")] public ushort Legs { get; set; }

        [JsonPropertyName("feet")] public ushort Feet { get; set; }

        [JsonPropertyName("head")] public ushort Head { get; set; }

        [JsonPropertyName("addons")] public ushort Addons { get; set; }
    }

    public class TargetchangeData
    {
        [JsonPropertyName("interval")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Interval { get; set; }

        [JsonPropertyName("chance")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Chance { get; set; }
    }

    public class StrategyData
    {
        [JsonPropertyName("attack")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Attack { get; set; }

        [JsonPropertyName("defense")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Defense { get; set; }
    }

    public class DefenseData
    {
        [JsonPropertyName("armor")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Armor { get; set; }

        [JsonPropertyName("defense")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Defense { get; set; }
    }

    public class Voice
    {
        [JsonPropertyName("sentence")] public string Sentence { get; set; }

        [JsonPropertyName("yell")] public bool Yell { get; set; }
    }

    public class VoicesData
    {
        [JsonPropertyName("interval")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Interval { get; set; }

        [JsonPropertyName("chance")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Chance { get; set; }

        [JsonPropertyName("sentences")] public List<Voice> Sentences { get; set; } = new List<Voice>();
    }

    public class ItemData
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("countmax")] public string Countmax { get; set; }

        [JsonPropertyName("chance")] public string Chance { get; set; }

        [JsonPropertyName("item")] public List<ItemData> Item { get; set; } = new List<ItemData>();
    }

    public class LootData
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Id { get; set; }

        [JsonPropertyName("countmax")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string CountMax { get; set; }

        [JsonPropertyName("chance")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Chance { get; set; }

        [JsonPropertyName("items")] public List<LootData> Items { get; set; } = new List<LootData>();
    }

    public class SummonData
    {
        [JsonPropertyName("maxSummons")] public int MaxSummons { get; set; }

        [JsonPropertyName("summons")] public List<MonsterSummonData> Summons { get; set; } = new List<MonsterSummonData>();
    }

    public class MonsterSummonData
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("interval")] public uint Interval { get; set; }

        [JsonPropertyName("chance")] public int Chance { get; set; }

        [JsonPropertyName("max")] public int Max { get; set; }
    }

    public class NumberToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return reader.GetInt32().ToString();
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString() ?? throw new JsonException("String value is null.");
            throw new JsonException("Invalid JSON format for a string representation of a number.");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (int.TryParse(value, out var number))
                writer.WriteNumberValue(number);
            else
                writer.WriteStringValue(value);
        }
    }
}