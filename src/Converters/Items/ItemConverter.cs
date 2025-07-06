using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Formatting = Newtonsoft.Json.Formatting;
using Converters.Helpers;

namespace Converters;

public static class ItemConverter
{
    public static void Convert(string path)
    {
        var inputFilePath = Path.Combine(path, "items.xml");
        var outputFilePath = Path.Combine(path, "items.json");

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine($"items.xml not found: {inputFilePath}");
            return;
        }

        var xml = File.ReadAllText(inputFilePath);
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        // Remove os '@' gerados por atributos XML ao converter para JSON
        var json = JsonConvert.SerializeXmlNode(doc.DocumentElement, Formatting.Indented, false);
        json = Regex.Replace(json, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase);

        var outputObject = new ItemFromjson().Convert(doc);

        var jsonSerialized = JsonSerializer.Serialize(outputObject, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        });

        jsonSerialized = JsonFormatter.UnescapeUnicode(jsonSerialized);

        if (!JsonValidator.IsValid(jsonSerialized))
        {
            Console.WriteLine("Error: Invalid JSON.");
            Console.ReadKey();
            return;
        }

        File.WriteAllText(outputFilePath, jsonSerialized);
        Console.WriteLine("Items conversion completed successfully!");
    }
}