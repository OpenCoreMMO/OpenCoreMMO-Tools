using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace XmlToJson.Standalone
{
    public class Monster
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Path:");
            var path = Console.ReadLine();
            var xmlFiles = Directory.GetFileSystemEntries(path, "*.xml", SearchOption.AllDirectories);
            
            var output = "C:/jsonoutput";


            var i = 0;
            foreach (var file in xmlFiles)
            {
                
                var xml = File.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

              

                string json = JsonConvert.SerializeXmlNode(doc.FirstChild.NextSibling, Newtonsoft.Json.Formatting.Indented, false);

                json = Regex.Replace(json, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase);

                if (doc.SelectSingleNode("spawns") != null)
                {
                    var spawns = new SpawnConverter().Convert(doc);
                    Save(path, file, output, JsonConvert.SerializeObject(spawns, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                }
                else if (doc.SelectSingleNode("monster") != null)
                {
                    var outputObject = new JsonToMonster().Convert(json, doc.FirstChild.NextSibling);


                    Save(path, file, output, JsonConvert.SerializeObject(outputObject, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                }

             
                Console.WriteLine($"{++i}/{xmlFiles.Length}");
            }

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
