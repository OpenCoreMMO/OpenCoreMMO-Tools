using Converters.Helpers;
using Converters.Spawn;
using System.IO;
using System.Linq;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Converters;

public static class SpawnConverter
{
    public static void Convert(string path)
    {
        var xmlFiles = Directory.GetFileSystemEntries(path, "*xml", SearchOption.AllDirectories);

        var i = 0;
        foreach (var file in xmlFiles)
        {
            var xml = File.ReadAllText(file);
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            if (doc.SelectSingleNode("/spawns") is not null)
            {
                var outputObject = new SpawnFromJson().Convert(doc).ToList();
                var outputPath = Path.Combine(file.Replace(".xml", ".json"));

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
            }

            Console.WriteLine($"Converted spawn {++i}/{xmlFiles.Length}");
        }

        Console.WriteLine("Spawns conversion completed successfully!");
    }
}