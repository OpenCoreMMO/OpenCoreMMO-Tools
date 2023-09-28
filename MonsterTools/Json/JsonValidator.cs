using System.Text.Json;

namespace MonsterTools.Json;

public static class JsonValidator
{
    public static bool IsValid(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            using var jsonDoc = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}