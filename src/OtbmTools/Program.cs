using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using Converters.Json;
using Converters.Monsters;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MonsterTools;

internal static class Program
{
    private static void Main(string[] args)
    {
        var xmlFiles =
            Directory.GetFileSystemEntries(Directory.GetCurrentDirectory(), "*xml", SearchOption.AllDirectories);

        var output = "./";

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
                var outputObject = new JsonToMonster().Convert(json, doc.FirstChild.NextSibling);

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
                    Console.WriteLine($"Invalid Json: {file}");
                    Console.ReadKey();
                    return;
                }

                Save(file.Replace("xml", "json"), output, jsonSerialized);
            }

            Console.WriteLine($"Converted {++i}/{xmlFiles.Length}");
        }

        Console.WriteLine("Done!");

        Console.ReadKey();
    }

    private static void Save(string file, string outputPath, string value)
    {
        File.WriteAllText(Path.Combine(outputPath, file), value);
    }
}