using System;
using System.Collections.Generic;
using System.Xml;

namespace Converters;

public class SpawnConverter
{
    public IEnumerable<SpawnOutput> Convert(XmlDocument doc)
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

            foreach (XmlNode creatureNode in spawnNode.ChildNodes)
            {
                var name = creatureNode.Attributes["name"].Value;
                if (!int.TryParse(creatureNode.Attributes["x"].Value, out var x)) continue;
                if (!int.TryParse(creatureNode.Attributes["y"].Value, out var y)) continue;
                if (!int.TryParse(creatureNode.Attributes["z"].Value, out var z)) continue;
                if (!int.TryParse(creatureNode.Attributes["spawntime"].Value, out var spawntime)) continue;

                if (string.IsNullOrWhiteSpace(name)) continue;

                var creature = new SpawnOutput.Creature
                {
                    Name = name,
                    X = x,
                    Y = y,
                    Spawntime = spawntime > ushort.MaxValue ? ushort.MaxValue : spawntime,
                    Z = z
                };

                if (creatureNode.Name.Equals("npc", StringComparison.InvariantCultureIgnoreCase))
                    spawn.Npcs.Add(creature);
                else if (creatureNode.Name.Equals("monster", StringComparison.InvariantCultureIgnoreCase))
                    spawn.Monsters.Add(creature);
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
    public List<Creature> Monsters { get; set; } = new();
    public List<Creature> Npcs { get; set; } = new();

    public class Creature
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Spawntime { get; set; }
    }
}