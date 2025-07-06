using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Converters.Helpers.JsonConverters;

public class FloatAsStringConverter : JsonConverter<float>
{
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return float.Parse(reader.GetString(), CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}
