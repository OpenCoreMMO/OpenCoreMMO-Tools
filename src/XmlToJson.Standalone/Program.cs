using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Converters;
using Converters.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Formatting = Newtonsoft.Json.Formatting;

namespace XmlToJson.Standalone
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Path:");
            var path = Console.ReadLine();
            var xmlFiles = Directory.GetFileSystemEntries(path, "*.xml", SearchOption.AllDirectories);

            var output = "C:/jsonoutput";


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
                    Save(path, file, output, JsonConvert.SerializeObject(spawns, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
                }
                else if (doc.SelectSingleNode("monster") != null)
                {
                    var outputObject = new JsonToMonster().Convert(json, doc.FirstChild.NextSibling);


                    Save(path, file, output, JsonConvert.SerializeObject(outputObject, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
                }


                Console.WriteLine($"{++i}/{xmlFiles.Length}");
            }

            Console.WriteLine($"Files saved in: {output}");
        }

        private static void Save(string rootPath, string path, string outputPath, string value)
        {
            path = path.Replace(rootPath, "");
            path = path.Replace("xml", "json");

            var resultPath = Path.Join(outputPath, path);

            Directory.CreateDirectory(Directory.GetParent(resultPath).FullName);
            File.WriteAllText(Path.Join(outputPath, path), value);
        }
    }
}