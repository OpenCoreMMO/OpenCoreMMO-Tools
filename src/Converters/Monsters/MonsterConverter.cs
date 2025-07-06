using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using System.IO;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Converters.Helpers;
using Converters.Monsters;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Formatting = Newtonsoft.Json.Formatting;

namespace Converters;

public static class MonsterConverter
{
    public static void Convert(string path)
    {
        var xmlFiles =
            Directory.GetFileSystemEntries(path, "*xml", SearchOption.AllDirectories);

        var i = 0;
        foreach (var file in xmlFiles)
        {
            var xml = File.ReadAllText(file);
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            if (doc?.FirstChild is null) continue;

            var json = JsonConvert.SerializeXmlNode(doc.FirstChild.NextSibling,
                Formatting.Indented, false);

            json = Regex.Replace(json, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase);

            if (doc.SelectSingleNode("monster") != null)
            {
                var outputObject = new MonsterFromJson().Convert(json, doc.FirstChild.NextSibling);
                var outputPath = Path.Combine(file.Replace(".xml", ".json"));

                var jsonSerialized = JsonSerializer.Serialize(outputObject,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                jsonSerialized = JsonFormatter.Format(jsonSerialized);

                if (!JsonValidator.IsValid(jsonSerialized))
                {
                    Console.WriteLine("Error: Invalid JSON.");
                    Console.ReadKey();
                    return;
                }

                File.WriteAllText(outputPath, jsonSerialized);
            }

            Console.WriteLine($"Converted monster {++i}/{xmlFiles.Length}");
        }

        Console.WriteLine("Monsters conversion completed successfully!");
    }
}