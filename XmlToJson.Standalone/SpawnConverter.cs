using System.Collections.Generic;
using System.Xml;

namespace XmlToJson.Standalone
{
    internal class SpawnConverter
    {
        internal IEnumerable<SpawnOutput> Convert(XmlDocument doc)
        {
            var spawnNodes = doc.SelectNodes("spawns/spawn");


            foreach (XmlNode spawnNode in spawnNodes)
            {
                var spawn = new SpawnOutput();

                if (!int.TryParse(spawnNode.Attributes["centerx"].Value, out var centerX)) continue;
                if (!int.TryParse(spawnNode.Attributes["centery"].Value, out var centerY)) continue;
                if (!int.TryParse(spawnNode.Attributes["centerz"].Value, out var centerZ)) continue;
                if (!int.TryParse(spawnNode.Attributes["radius"].Value, out var radius)) continue;


                spawn.Centerx = centerX;
                spawn.Centery = centerY;
                spawn.Centerz = centerZ;
                spawn.Radius = radius;

                foreach (XmlNode monsterNode in spawnNode.ChildNodes)
                {
                    var name = monsterNode.Attributes["name"].Value;
                    if (!int.TryParse(monsterNode.Attributes["x"].Value, out var x)) continue;
                    if (!int.TryParse(monsterNode.Attributes["y"].Value, out var y)) continue;
                    if (!int.TryParse(monsterNode.Attributes["z"].Value, out var z)) continue;
                    if (!int.TryParse(monsterNode.Attributes["spawntime"].Value, out var spawntime)) continue;

                    if (string.IsNullOrWhiteSpace(name)) continue;

                    spawn.Monsters.Add(new SpawnOutput.Monster
                    {
                        Name = name,
                        X = x,
                        Y = y,
                        Spawntime = spawntime,
                        Z = z
                    });
                }

                yield return spawn;
            }
        }
    }


    public class SpawnOutput
    {
        public int Centerx { get; set; }
        public int Centery { get; set; }
        public int Centerz { get; set; }
        public int Radius { get; set; }
        public List<Monster> Monsters { get; set; } = new List<Monster>();

        public class Monster
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int Spawntime { get; set; }
        }
    }
}