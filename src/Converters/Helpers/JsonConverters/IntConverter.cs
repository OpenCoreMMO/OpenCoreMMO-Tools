using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Converters.Helpers.JsonConverters;

public class IntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => int.Parse(reader.GetString(), CultureInfo.InvariantCulture),
            JsonTokenType.Number => reader.GetInt32(),
            _ => throw new JsonException("Invalid JSON token for int value")
        };
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}