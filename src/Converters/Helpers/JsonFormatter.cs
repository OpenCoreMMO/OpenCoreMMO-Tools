using System.Linq;
using System.Text;
using System.Text.Json;

namespace Converters.Helpers;

public static class JsonFormatter
{
    public static string UnescapeUnicode(this string json)
    {
        // Substitui códigos Unicode por seus caracteres reais
        return json
            .Replace("\\u0027", "'"); // '
    }

    public static string Format(string json, bool breakLine = true)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var sb = new StringBuilder();

        var formatted = IndentJson(jsonDocument.RootElement, sb, breakLine);
        return formatted.UnescapeUnicode();
    }

    private static string IndentJson(JsonElement element, StringBuilder sb, bool breakLine, int level = 0)
    {
        level++;

        var breakLikeSpace = string.Join("", Enumerable.Range(0, level).Select(_ => "  "));

        var space = breakLine ? breakLikeSpace : " ";

        sb.Append("{");

        var hasAnyProperty = false;
        foreach (var property in element.EnumerateObject())
        {
            hasAnyProperty = true;
            if (breakLine) sb.Append("\r\n");
            sb.Append(@$"{space}""" + property.Name + "\": ");

            AppendValue(property, sb, level);

            sb.Append(',');
        }

        if (hasAnyProperty) sb.Remove(sb.Length - 1, 1);

        if (breakLine) sb.Append("\r\n");

        if (breakLine) sb.Append("}");
        else sb.Append($"{space}}}");

        return sb.ToString();
    }

    private static void AppendValue(JsonProperty property, StringBuilder sb, int level)
    {
        switch (property.Value.ValueKind)
        {
            case JsonValueKind.Array:
                IndentArray(sb, property.Value, level);
                break;
            case JsonValueKind.Object:
                IndentJson(property.Value, sb, false, level);
                break;
            default:
                sb.Append(property.Value.GetRawText());
                break;
        }
    }

    private static void IndentArray(StringBuilder sb, JsonElement value, int level)
    {
        sb.Append("[");

        var count = value.GetArrayLength();

        var breakLikeSpace = "   " + string.Join("", Enumerable.Range(0, level).Select(_ => "  "));

        for (var i = 0; i < count; i++)
        {
            if (count > 1)
            {
                sb.Append("\r\n");
                sb.Append($"{breakLikeSpace}");
            }

            IndentJson(value[i], sb, false, level);
            sb.Append(",");
        }

        if (count > 0) sb.Remove(sb.Length - 1, 1);

        if (count == 1)
        {
            sb.Append("]");
            return;
        }

        sb.Append(count == 0 ? "]" : "\r\n  ]");
    }
}