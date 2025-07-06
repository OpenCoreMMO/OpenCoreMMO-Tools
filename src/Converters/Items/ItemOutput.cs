using Converters.Helpers.JsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Converters.Items;

public class ItemOutput
{
    [JsonConverter(typeof(StringConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }

    [JsonConverter(typeof(StringConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FromId { get; set; }

    [JsonConverter(typeof(StringConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ToId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Plural { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Article { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<ItemAttributeOutput>? Attributes { get; set; }
}

public class ItemAttributeOutput
{
    public string Key { get; set; }
    public string Value { get; set; }
}
