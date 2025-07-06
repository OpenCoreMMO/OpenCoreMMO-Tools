using Converters.Vocations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Converters.JsonConverters;

public class SkillConverter : JsonConverter<Dictionary<SkillType, float>>
{
    public override Dictionary<SkillType, float> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token.");

        var list = new List<Dictionary<string, string>>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var item = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
            if (item != null) list.Add(item);
        }

        return list.ToDictionary(
            x => ParseSkillName(x["name"]),
            x => float.Parse(x["multiplier"], CultureInfo.InvariantCulture.NumberFormat));
    }

    private static SkillType ParseSkillName(string skillName)
    {
        return skillName switch
        {
            "fist" => SkillType.Fist,
            "axe" => SkillType.Axe,
            "sword" => SkillType.Sword,
            "club" => SkillType.Club,
            "shielding" => SkillType.Shielding,
            "fishing" => SkillType.Fishing,
            "distance" => SkillType.Distance,
            "magic" => SkillType.Magic,
            "level" => SkillType.Level,
            "speed" => SkillType.Speed,
            _ => throw new ArgumentOutOfRangeException(nameof(skillName), skillName, null)
        };
    }
    private static string GetSkillName(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Fist => "fist",
            SkillType.Axe => "axe",
            SkillType.Sword => "sword",
            SkillType.Club => "club",
            SkillType.Shielding => "shielding",
            SkillType.Fishing => "fishing",
            SkillType.Distance => "distance",
            SkillType.Magic => "magic",
            SkillType.Level => "level",
            SkillType.Speed => "speed",
            _ => throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null)
        };
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<SkillType, float> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var kvp in value)
        {
            writer.WriteStartObject();

            writer.WriteString("name", GetSkillName(kvp.Key));
            writer.WriteString("multiplier", kvp.Value.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}