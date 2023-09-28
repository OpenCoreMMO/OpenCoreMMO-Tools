using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Converters.Monsters;

public class JsonToMonster
{
    private static readonly HashSet<string> SupportedProperties = new()
    {
        "name",
        "chance",
        "interval",
        "attack",
        "skill",
        "min",
        "max",
        "range",
        "radius",
        "length",
        "spread",
        "target",
        "areaEffect",
        "shootEffect",
        "attribute"
    };

    private static HashSet<string> CombatTypes { get; } = new()
    {
        "physical",
        "earth",
        "fire",
        "energy",
        "ice",
        "poison",
        "terra strike",
        "death",
        "holy"
    };

    public MonsterOutput Convert(string json, XmlNode xml)
    {
        var monster = new MonsterOutput();

        var obj = ((JObject)JsonConvert.DeserializeObject(json))?["monster"];

        monster.Name = obj.Value<string>("name");
        monster.Description = obj.Value<string>("nameDescription");
        monster.Race = obj.Value<string>("race");
        monster.Experience = obj.Value<int>("experience");
        monster.Speed = obj.Value<int>("speed");
        monster.ManaCost = obj.Value<int>("manacost");

        var health = obj.Value<JObject>("health");

        monster.MaxHealth = health.Value<int>("max");
        monster.Health = health.Value<int>("now");

        var look = obj.Value<JObject>("look");
        if (look != null)
        {
            monster.Corpse = look.Value<int>("corpse");
            monster.Outfit.Type = look.Value<int>("type");
            monster.Outfit.Head = look.Value<int>("head");
            monster.Outfit.Body = look.Value<int>("body");
            monster.Outfit.Legs = look.Value<int>("legs");
            monster.Outfit.Feet = look.Value<int>("feet");
        }

        var targetChange = obj.Value<JObject>("targetchange");

        if (targetChange != null && targetChange.Value<int>("chance") > 0)
        {
            monster.TargetChange ??= new MonsterOutput.TargetChangeData();
            monster.TargetChange.Interval = targetChange.Value<int>("interval");
            monster.TargetChange.Chance = targetChange.Value<int>("chance");
        }

        var flags = obj["flags"]["flag"] as JArray;

        foreach (var item in flags.Children<JObject>())
        foreach (var prop in item.Properties())
            monster.Flags.TryAdd(prop.Name, prop.Value?.Value<int?>() ?? default);

        ConvertAttack(xml, monster);

        ConvertDefenses(xml, monster, obj);

        var lootNode = xml.SelectNodes("loot/item");


        foreach (XmlNode lootItemNode in lootNode)
        {
            monster.Loot ??= new List<MonsterOutput.LootData>();
            monster.Loot.Add(ConvertLoot(monster, lootItemNode));
        }

        monster.Loot = monster.Loot?.OrderBy(x => x.Name).ToList();

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

        int.TryParse(summonNode.Attributes["maxSummons"].Value, out var maxSummons);

        if (maxSummons == 0) return;

        monster.Summon ??= new MonsterOutput.SummonData();
        monster.Summon.MaxSummons = maxSummons;

        foreach (XmlNode summonChild in summonNode.ChildNodes)
        {
            if (summonChild.Attributes.Count == 0) continue;

            var name = summonChild.Attributes["name"].Value;
            var interval = int.TryParse(summonChild.Attributes["interval"]?.Value, out var intervalValue)
                ? intervalValue
                : 2000;
            var chance = int.TryParse(summonChild.Attributes["chance"]?.Value, out var chanceValue)
                ? chanceValue
                : 0;
            var max = int.TryParse(summonChild.Attributes["max"]?.Value, out var maxValue) ? maxValue : 0;

            if (string.IsNullOrWhiteSpace(name)) continue;

            monster.Summon.Summons.Add(new MonsterOutput.SummonCreatureData
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
            monster.Immunities ??= new Dictionary<string, int>();

            if (immunityNode.Attributes.Count == 0) continue;

            var name = immunityNode.Attributes[0]?.Name;
            var value = int.TryParse(immunityNode.Attributes[0]?.Value, out var attrValue) ? attrValue : 0;

            if (value == 0) continue;

            if (string.IsNullOrWhiteSpace(name)) continue;

            monster.Immunities.Add(name, value);
        }
    }

    private static void ConvertElements(XmlNode xml, MonsterOutput monster)
    {
        var elementsNode = xml.SelectNodes("elements/element");

        foreach (XmlNode elementNode in elementsNode)
        {
            monster.Elements ??= new Dictionary<string, int>();
            if (elementNode.Attributes.Count == 0) continue;

            var name = elementNode.Attributes[0]?.Name;
            var value = int.TryParse(elementNode.Attributes[0]?.Value, out var attrValue) ? attrValue : 0;

            if (value == 0) continue;

            if (string.IsNullOrWhiteSpace(name)) continue;

            monster.Elements.TryAdd(name, value);
        }
    }

    private static void ConvertVoices(XmlNode xml, MonsterOutput monster)
    {
        var voiceNode = xml.SelectSingleNode("voices");

        if (voiceNode != null)
        {
            monster.Voices = new MonsterOutput.VoicesData
            {
                Interval = int.TryParse(voiceNode.Attributes["interval"]?.Value, out var interval)
                    ? interval
                    : 5000,
                Chance = int.TryParse(voiceNode.Attributes["chance"]?.Value, out var chance) ? chance : 10
            };

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
            Chance = int.TryParse(lootItemNode.Attributes["chance"]?.Value, out var chance) ? chance : 0,
            Id = int.TryParse(lootItemNode.Attributes["id"]?.Value, out var id) ? id : null,
            Name = lootItemNode.Attributes["name"]?.Value,
            Countmax = int.TryParse(lootItemNode.Attributes["countmax"]?.Value, out var countMax) ? countMax : 1
        };

        if (lootItemNode.HasChildNodes)
        {
            loot.Items ??= new List<MonsterOutput.LootData>();
            foreach (XmlNode lootChild in lootItemNode.SelectNodes("item"))
                loot.Items.Add(ConvertLoot(monster, lootChild));
        }

        return loot;
    }

    private static void ConvertDefenses(XmlNode xml, MonsterOutput monster, JToken obj)
    {
        var defense = obj.Value<JObject>("defenses");

        if (defense is null) return;

        var node = xml.SelectSingleNode("defenses");

        monster.Defense.Armor = defense.Value<int>("armor");

        int.TryParse(node.Attributes["defense"]?.Value, out var defenseValue);
        monster.Defense.Defense = defenseValue;

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

        monster.Defenses = defenseList.Any() ? defenseList : null;
    }

    private static void ConvertAttack(XmlNode xml, MonsterOutput monster)
    {
        var attackNodes = xml.SelectNodes("attacks/attack");

        var attackList = new List<MonsterOutput.Attack>();

        foreach (XmlNode attackNode in attackNodes)
        {
            var xmlValues = ParseAttacksToDictionary(attackNode, out var attack);

            xmlValues.TryGetValue("name", out var name);

            xmlValues.TryGetValue("chance", out var chance);
            xmlValues.TryGetValue("interval", out var interval);
            xmlValues.TryGetValue("attack", out var attackValue);
            xmlValues.TryGetValue("skill", out var skill);
            xmlValues.TryGetValue("min", out var minDamage);
            xmlValues.TryGetValue("max", out var maxDamage);
            xmlValues.TryGetValue("range", out var range);
            xmlValues.TryGetValue("radius", out var radius);
            xmlValues.TryGetValue("length", out var length);

            xmlValues.TryGetValue("spread", out var spread);
            xmlValues.TryGetValue("target", out var needsTarget);

            xmlValues.TryGetValue("poison", out var poison);
            xmlValues.TryGetValue("fire", out var fire);

            //handle typos
            if (length is null) xmlValues.TryGetValue("lenght", out length);
            if (skill is null) xmlValues.TryGetValue("skil", out skill);

            minDamage = minDamage?.ToString().Replace("-", "");
            maxDamage = maxDamage?.ToString().Replace("-", "");

            attack.Name = name as string;
            attack.Chance = chance is null ? 100 : int.Parse(chance as string);
            attack.Interval = interval is null ? null : int.Parse(interval as string);
            attack.MinDamage = minDamage is null ? null : -int.Parse(minDamage as string);
            attack.MaxDamage = maxDamage is null ? null : -int.Parse(maxDamage as string);
            attack.Range = range is null ? null : int.Parse(range as string);
            attack.Radius = radius is null ? null : int.Parse(radius as string);
            attack.Length = length is null ? null : int.Parse(length as string);
            attack.Spread = spread is null ? null : int.Parse(spread as string);
            attack.NeedTarget = needsTarget is null ? null : needsTarget as string == "0";

            if (CombatTypes.Contains(xmlValues["name"]))
            {
                attack.Name = "combat";
                attack.DamageType = name as string;
            }

            foreach (var key in xmlValues.Keys)
            {
                if (SupportedProperties.Contains(key)) continue;

                attack.Extra ??= new Dictionary<string, object>();
                attack.Extra.Add(key, xmlValues[key]);
            }

            AddCondition(poison, fire, attack);

            var attackAttributes = attackNode.SelectNodes("attribute");

            foreach (XmlNode defAttrNode in attackAttributes)
            {
                var key = defAttrNode.Attributes["key"].Value;
                var value = defAttrNode.Attributes["value"].Value;

                if (key.ToLowerInvariant() == "shooteffect")
                {
                    attack.ShootEffect = value;
                    continue;
                }

                if (key.ToLowerInvariant() == "areaeffect")
                {
                    attack.Effect = value;
                    continue;
                }

                attack.Extra.Add(key, value);
            }

            attackList.Add(attack);
        }

        monster.Attacks = attackList.Any() ? attackList.OrderBy(x => x.Name).ToList() : null;
    }

    private static void AddCondition(object poison, object fire, MonsterOutput.Attack attack)
    {
        if (poison is not null || fire is not null)
        {
            var type = poison is not null ? "poison" : "fire";
            var totalDamage = poison ?? fire;
            attack.Condition = new MonsterOutput.Attack.AttackCondition
            {
                Interval = 4000,
                Type = type,
                TotalDamage = totalDamage is null ? null : int.Parse(totalDamage as string)
            };
        }
    }

    private static Dictionary<string, object> ParseAttacksToDictionary(XmlNode attackNode,
        out MonsterOutput.Attack attack)
    {
        var xmlValues = new Dictionary<string, object>();

        attack = new MonsterOutput.Attack();

        foreach (XmlAttribute attr in attackNode.Attributes) xmlValues.Add(attr.Name, attr.Value);

        return xmlValues;
    }
}