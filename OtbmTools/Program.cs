using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace OtbmTools;

internal class Program
{
    private static void Main(string[] args)
    {
        var xmlFiles = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory(), "*-spawn.xml",
            SearchOption.AllDirectories);

        var output = "./";

        var i = 0;
        foreach (var file in xmlFiles)
        {
            var xml = File.ReadAllText(file);
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(doc.FirstChild.NextSibling, Formatting.Indented, false);

            json = Regex.Replace(json, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase);

            if (doc.SelectSingleNode("spawns") != null)
            {
                var spawns = new SpawnConverter().Convert(doc);
                Save(file.Replace("xml", "json"), output, JsonConvert.SerializeObject(spawns, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
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