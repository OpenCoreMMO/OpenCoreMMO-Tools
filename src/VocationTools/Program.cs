using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Converters.Vocations;

namespace VocationTools;

internal static class Program
{
    private static void Main(string[] args)
    {
        var inputFilePath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml", SearchOption.TopDirectoryOnly)[0];

        var xml = File.ReadAllText(inputFilePath);
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var outputObject = new JsonToVocation().Convert(doc);

        var jsonSerialized = JsonSerializer.Serialize(outputObject,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

        Save("vocations.json", jsonSerialized);
        Console.WriteLine("Vocations converted successfully.");
        Console.ReadKey();
    }

    private static void Save(string file, string content)
    {
        File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), file), content);
    }
}
