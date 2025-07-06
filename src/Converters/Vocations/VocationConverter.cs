using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using System.IO;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Converters.Vocations;
using Converters.Helpers;

namespace Converters;

public static class VocationConverter
{
    public static void Convert(string path)
    {
        var inputFilePath = Directory.GetFiles(path, "vocations.xml", SearchOption.TopDirectoryOnly)[0];

        if (string.IsNullOrEmpty(inputFilePath))
        {
            Console.WriteLine($"vocations.xml not found: {inputFilePath}");
            Console.ReadKey();
            return;
        }

        var xml = File.ReadAllText(inputFilePath);
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var outputObject = new VocationFromJson().Convert(doc);
        var outputPath = Path.Combine(inputFilePath.Replace(".xml", ".json"));

        var jsonSerialized = JsonSerializer.Serialize(outputObject,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

        jsonSerialized = JsonFormatter.UnescapeUnicode(jsonSerialized);

        if (!JsonValidator.IsValid(jsonSerialized))
        {
            Console.WriteLine("Error: Invalid JSON.");
            Console.ReadKey();
            return;
        }

        File.WriteAllText(outputPath, jsonSerialized);
        Console.WriteLine("Vocation conversion completed successfully!");
    }
}