using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using XmlToJson.Standalone;

namespace MonsterTools
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlFiles = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory(), "*xml", SearchOption.AllDirectories);

            var output = "./";

            var i = 0;
            foreach (var file in xmlFiles)
            {

                var xml = File.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                string json = JsonConvert.SerializeXmlNode(doc.FirstChild.NextSibling, Newtonsoft.Json.Formatting.Indented, false);

                json = Regex.Replace(json, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase);

                if (doc.SelectSingleNode("monster") != null)
                {
                    var outputObject = new JsonToMonster().Convert(json, doc.FirstChild.NextSibling);

                    Save(file.Replace("xml", "json"), output, JsonConvert.SerializeObject(outputObject, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                }

                Console.WriteLine($"Converted {++i}/{xmlFiles.Length}");
            }
            Console.WriteLine($"Done!");

            Console.ReadKey();
        }

        private static void Save(string file, string outputPath, string value)
        {

            File.WriteAllText(Path.Combine(outputPath, file), value);
        }
    }
}
