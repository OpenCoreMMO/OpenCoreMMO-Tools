using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Converters.Spawn;

public class SpawnFromJson
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

            foreach (XmlNode node in spawnNode.ChildNodes)
            {
                if (node.Name.Equals("monster", StringComparison.InvariantCultureIgnoreCase) ||
                    node.Name.Equals("npc", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Formato 1: instância direta
                    var nameAttr = node.Attributes["name"];
                    if (nameAttr == null) continue;

                    if (!int.TryParse(node.Attributes["x"]?.Value, out var x)) continue;
                    if (!int.TryParse(node.Attributes["y"]?.Value, out var y)) continue;
                    if (!int.TryParse(node.Attributes["z"]?.Value, out var z)) continue;
                    if (!int.TryParse(node.Attributes["spawntime"]?.Value, out var spawntime)) continue;

                    var creature = new SpawnOutput.Creature
                    {
                        Name = nameAttr.Value,
                        X = x,
                        Y = y,
                        Z = z,
                        Spawntime = spawntime > ushort.MaxValue ? ushort.MaxValue : spawntime
                    };

                    if (node.Name.Equals("npc", StringComparison.InvariantCultureIgnoreCase))
                        spawn.Npcs.Add(creature);
                    else
                        spawn.Monsters.Add(creature);
                }
                else if (node.Name.Equals("monsters", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Formato 2: grupo com múltiplas opções
                    if (!int.TryParse(node.Attributes["x"]?.Value, out var x)) continue;
                    if (!int.TryParse(node.Attributes["y"]?.Value, out var y)) continue;
                    if (!int.TryParse(node.Attributes["z"]?.Value, out var z)) continue;
                    if (!int.TryParse(node.Attributes["spawntime"]?.Value, out var spawntime)) continue;

                    foreach (XmlNode monsterOption in node.ChildNodes)
                    {
                        if (monsterOption.Name != "monster") continue;

                        var nameAttr = monsterOption.Attributes["name"];
                        if (nameAttr == null) continue;

                        var creature = new SpawnOutput.Creature
                        {
                            Name = nameAttr.Value,
                            X = x,
                            Y = y,
                            Z = z,
                            Spawntime = spawntime > ushort.MaxValue ? ushort.MaxValue : spawntime
                        };

                        spawn.Monsters.Add(creature);
                    }
                }
            }

            if (!spawn.Monsters.Any())
                spawn.Monsters = null;
            if (!spawn.Npcs.Any())
                spawn.Npcs = null;

            yield return spawn;
        }
    }
}
