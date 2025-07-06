using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Converters.Monsters;


public class JsonToMonster
{
    public MonsterOutput Convert(string json, XmlNode xml)
    {
        var monster = new MonsterOutput();

        var obj = ((JObject)JsonConvert.DeserializeObject(json))["monster"];

        monster.Name = obj.Value<string>("name");
        monster.NameDescription = obj.Value<string>("nameDescription");
        monster.Race = obj.Value<string>("race");
        monster.Experience = obj.Value<uint>("experience");
        monster.Speed = obj.Value<ushort>("speed");
        monster.ManaCost = obj.Value<ushort>("manacost");

        var health = obj.Value<JObject>("health");

        monster.Health.Max = health.Value<uint>("max");
        monster.Health.Now = health.Value<uint>("now");

        var look = obj.Value<JObject>("look");
        if (look != null)
        {
            monster.Look.Corpse = look.Value<ushort>("corpse");
            monster.Look.Type = look.Value<ushort>("type");
            monster.Look.Head = look.Value<ushort>("head");
            monster.Look.Body = look.Value<ushort>("body");
            monster.Look.Legs = look.Value<ushort>("legs");
            monster.Look.Feet = look.Value<ushort>("feet");
        }

        var targetChange = obj.Value<JObject>("targetchange");
        if (targetChange != null)
        {
            monster.TargetChange.Interval = targetChange.Value<string>("interval");
            monster.TargetChange.Chance = targetChange.Value<string>("chance");
        }

        var strategy = obj.Value<JObject>("strategy");
        if (strategy != null)
        {
            monster.Strategy.Attack = strategy.Value<string>("attack");
            monster.Strategy.Defense = strategy.Value<string>("defense");
        }

        var flags = obj["flags"]["flag"] as JArray;

        foreach (var item in flags.Children<JObject>())
            foreach (var prop in item.Properties())
                monster.Flags.TryAdd(prop.Name, prop.Value?.Value<ushort?>() ?? default);

        ConvertAttack(xml, monster);

        ConvertDefenses(xml, monster, obj);

        var lootNode = xml.SelectNodes("loot/item");


        foreach (XmlNode lootItemNode in lootNode) monster.Loot.Add(ConvertLoot(monster, lootItemNode));

        ConvertVoices(xml, monster);
        ConvertElements(xml, monster);

        ConvertImmunities(xml, monster);

        ConvertSummon(xml, monster);

        return monster;
    }

    private static void ConvertSummon(XmlNode xml, MonsterOutput monster)
    {
        var summonNode = xml.SelectSingleNode("summons");

        if (summonNode is null) return;
        monster.Summon.MaxSummons = int.TryParse(summonNode.Attributes["maxSummons"].Value, out var maxSummons)
            ? maxSummons
            : 1;

        foreach (XmlNode summonChild in summonNode.ChildNodes)
        {
            if (summonChild.Attributes.Count == 0) continue;

            var name = summonChild.Attributes["name"].Value;
            var interval = uint.TryParse(summonChild.Attributes["interval"]?.Value, out var intervalValue)
                ? intervalValue
                : 2000;
            var chance = int.TryParse(summonChild.Attributes["chance"]?.Value, out var chanceValue)
                ? chanceValue
                : 0;
            var max = int.TryParse(summonChild.Attributes["max"]?.Value, out var maxValue) ? maxValue : 0;

            if (string.IsNullOrWhiteSpace(name)) continue;

            monster.Summon.Summons.Add(new MonsterOutput.MonsterSummonData
            {
                Chance = chance,
                Interval = interval,
                Max = max,
                Name = name
            });
        }
    }

    private static void ConvertImmunities(XmlNode xml, MonsterOutput monster)
    {
        var immunityNodes = xml.SelectNodes("immunities/immunity");

        foreach (XmlNode immunityNode in immunityNodes)
        {
            if (immunityNode.Attributes.Count == 0) continue;

            var name = immunityNode.Attributes[0]?.Name;
            var value = int.TryParse(immunityNode.Attributes[0]?.Value, out var attrValue) ? attrValue : 0;

            if (value == 0) continue;

            if (string.IsNullOrWhiteSpace(name)) continue;

            monster.Immunities.Add(name, (byte)value);
        }
    }

    private static void ConvertElements(XmlNode xml, MonsterOutput monster)
    {
        var elementsNode = xml.SelectNodes("elements/element");

        foreach (XmlNode elementNode in elementsNode)
        {
            if (elementNode.Attributes.Count == 0) continue;

            var name = elementNode.Attributes[0]?.Name;
            var value = int.TryParse(elementNode.Attributes[0]?.Value, out var attrValue) ? attrValue : 0;

            if (value == 0) continue;

            if (string.IsNullOrWhiteSpace(name)) continue;

            monster.Elements.TryAdd(name, (sbyte)value);
        }
    }

    private static void ConvertVoices(XmlNode xml, MonsterOutput monster)
    {
        var voiceNode = xml.SelectSingleNode("voices");

        if (voiceNode != null)
        {
            monster.Voices.Interval = voiceNode.Attributes["interval"]?.Value is string interval
                ? interval
                : "5000";

            monster.Voices.Chance = voiceNode.Attributes["chance"]?.Value is string chance ? chance : "10";

            foreach (XmlNode voice in voiceNode.ChildNodes)
                monster.Voices.Sentences.Add(new MonsterOutput.Voice
                {
                    Sentence = voice.Attributes["sentence"].Value,
                    Yell = bool.TryParse(voice.Attributes["yell"]?.Value, out var yell) ? yell : false
                });
        }
    }

    private static MonsterOutput.LootData ConvertLoot(MonsterOutput monster, XmlNode lootItemNode)
    {
        var loot = new MonsterOutput.LootData
        {
            Chance = lootItemNode.Attributes["chance"]?.Value is string chance ? chance : "0",
            Id = lootItemNode.Attributes["id"]?.Value is string id ? id : "0",
            CountMax = lootItemNode.Attributes["countmax"]?.Value is string countmax ? countmax : "1",
        };

        if (lootItemNode.HasChildNodes)
            foreach (XmlNode lootChild in lootItemNode.SelectNodes("item"))
                loot.Items.Add(ConvertLoot(monster, lootChild));

        return loot;
    }

    private static void ConvertDefenses(XmlNode xml, MonsterOutput monster, JToken obj)
    {
        var defense = obj.Value<JObject>("defenses");

        if (defense is null) return;

        var node = xml.SelectSingleNode("defenses");

        monster.Defense.Armor = defense.Value<string>("armor");

        monster.Defense.Defense = node.Attributes["defense"]?.Value is string defence ? defence : "0";

        var defenseNodes = xml.SelectNodes("defenses/defense");

        var defenseList = new List<Dictionary<string, object>>();

        foreach (XmlNode defenseNode in defenseNodes)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (XmlAttribute attr in defenseNode.Attributes) dictionary.Add(attr.Name, attr.Value);

            var defenseAttributes = defenseNode.SelectNodes("attribute");

            var attributes = new List<Dictionary<string, object>>();
            foreach (XmlNode defAttrNode in defenseAttributes)
                attributes.Add(new Dictionary<string, object>
                    { { defAttrNode.Attributes["key"].Value, defAttrNode.Attributes["value"].Value } });
            if (attributes.Any()) dictionary.Add("attributes", attributes);

            defenseList.Add(dictionary);
        }

        monster.Defenses = defenseList;
    }

    private static void ConvertAttack(XmlNode xml, MonsterOutput monster)
    {
        var attackNodes = xml.SelectNodes("attacks/attack");

        // if (attackNodes is null) return;


        var attackList = new List<Dictionary<string, object>>();

        foreach (XmlNode attackNode in attackNodes)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (XmlAttribute attr in attackNode.Attributes) dictionary.Add(attr.Name, attr.Value);

            var attackAttributes = attackNode.SelectNodes("attribute");

            var attributes = new List<Dictionary<string, object>>();
            foreach (XmlNode defAttrNode in attackAttributes)
                attributes.Add(new Dictionary<string, object>
                    { { defAttrNode.Attributes["key"].Value, defAttrNode.Attributes["value"].Value } });
            if (attributes.Any()) dictionary.Add("attributes", attributes);

            attackList.Add(dictionary);
        }

        monster.Attacks = attackList;
    }
}