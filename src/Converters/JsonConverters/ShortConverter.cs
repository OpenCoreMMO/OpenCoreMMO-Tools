using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Converters.JsonConverters;

public class ShortConverter : JsonConverter<short>
{
    public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number) return reader.GetInt16();
        if (reader.TokenType == JsonTokenType.String && short.TryParse(reader.GetString(), out var result))
            return result;

        throw new JsonException("Invalid value for short");
    }

    public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}