using Converters;

Console.WriteLine("TFS Path:");
var tfsPath = Console.ReadLine();

var tfsDataPath = Path.Join(tfsPath, "data");

var tfsItemsPath = Path.Join(tfsDataPath, "items");
ItemConverter.Convert(tfsItemsPath);

var tfsMonsterPath = Path.Join(tfsDataPath, "monster");
MonsterConverter.Convert(tfsMonsterPath);

var tfsWorldPath = Path.Join(tfsDataPath, "world");
SpawnConverter.Convert(tfsWorldPath);

var tfsVocationPath = Path.Join(tfsDataPath, "XML");
VocationConverter.Convert(tfsVocationPath);