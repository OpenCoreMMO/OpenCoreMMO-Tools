using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Converters.Spawn;

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
