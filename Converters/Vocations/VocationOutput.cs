using Converters.JsonConverters;
using NeoServer.Loaders.Converts;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Converters.Vocations;

public class VocationOutput
{
    public byte Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Inspect { get; set; }

    [JsonConverter(typeof(UshortConverter))]
    public new ushort GainCap { get; set; }

    [JsonConverter(typeof(UshortConverter))]
    public new ushort GainHp { get; set; }

    [JsonConverter(typeof(UshortConverter))]
    public ushort GainMana { get; set; }

    [JsonConverter(typeof(ByteConverter))] public new byte GainHpTicks { get; set; }

    [JsonConverter(typeof(ByteConverter))] public new byte GainManaTicks { get; set; }

    [JsonConverter(typeof(UshortConverter))]
    public new ushort GainHpAmount { get; set; }

    [JsonConverter(typeof(UshortConverter))]
    public new ushort GainManaAmount { get; set; }

    [JsonConverter(typeof(FloatAsStringConverter))]
    public float ManaMultiplier { get; set; }

    [JsonConverter(typeof(ShortConverter))]
    public new short AttackSpeed { get; set; }

    [JsonConverter(typeof(UshortConverter))]
    public new ushort BaseSpeed { get; set; }

    [JsonConverter(typeof(ByteConverter))] public new byte SoulMax { get; set; }

    [JsonConverter(typeof(IntConverter))]
    public new int GainSoulTicks { get; set; }

    public string FromVoc { get; set; }

    public VocationFormula Formula { get; set; }

    [JsonConverter(typeof(SkillConverter))]
    public new Dictionary<SkillType, float> Skills { get; set; }
}

public class VocationFormula
{
    public float MeleeDamage { get; set; }
    public float DistDamage { get; set; }
    public float Defense { get; set; }
    public float Armor { get; set; }
}

public enum SkillType : byte
{
    Fist = 0,
    Club = 1,
    Sword = 2,
    Axe = 3,
    Distance = 4,
    Shielding = 5,
    Fishing = 6,
    Magic = 7,
    Level = 8,
    Speed = 9,
    None
}