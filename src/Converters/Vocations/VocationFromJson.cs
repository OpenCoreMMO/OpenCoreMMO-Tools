using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Converters.Vocations;

public class VocationFromJson
{
    public List<VocationOutput> Convert(XmlDocument doc)
    {
        var vocations = new List<VocationOutput>();

        var vocationNodes = doc.SelectNodes("/vocations/vocation");

        foreach (XmlNode vocNode in vocationNodes)
        {
            var vocation = new VocationOutput
            {
                Id = byte.Parse(vocNode.Attributes["id"].Value),
                Name = vocNode.Attributes["name"].Value,
                Description = vocNode.Attributes["description"].Value,
                GainCap = ushort.Parse(vocNode.Attributes["gaincap"].Value),
                GainHp = ushort.Parse(vocNode.Attributes["gainhp"].Value),
                GainMana = ushort.Parse(vocNode.Attributes["gainmana"].Value),
                GainHpTicks = byte.Parse(vocNode.Attributes["gainhpticks"].Value),
                GainHpAmount = ushort.Parse(vocNode.Attributes["gainhpamount"].Value),
                GainManaTicks = byte.Parse(vocNode.Attributes["gainmanaticks"].Value),
                GainManaAmount = ushort.Parse(vocNode.Attributes["gainmanaamount"].Value),
                ManaMultiplier = float.Parse(vocNode.Attributes["manamultiplier"].Value, CultureInfo.InvariantCulture),
                AttackSpeed = short.Parse(vocNode.Attributes["attackspeed"].Value),
                SoulMax = byte.Parse(vocNode.Attributes["soulmax"].Value),
                GainSoulTicks = int.Parse(vocNode.Attributes["gainsoulticks"].Value),
                FromVoc = vocNode.Attributes["fromvoc"].Value,
                BaseSpeed = 220
            };

            var formulaNode = vocNode.SelectSingleNode("formula");
            if (formulaNode != null)
            {
                vocation.Formula = new VocationFormula
                {
                    MeleeDamage = float.Parse(formulaNode.Attributes["meleeDamage"].Value, CultureInfo.InvariantCulture),
                    DistDamage = float.Parse(formulaNode.Attributes["distDamage"].Value, CultureInfo.InvariantCulture),
                    Defense = float.Parse(formulaNode.Attributes["defense"].Value, CultureInfo.InvariantCulture),
                    Armor = float.Parse(formulaNode.Attributes["armor"].Value, CultureInfo.InvariantCulture)
                };
            }

            var skills = new Dictionary<SkillType, float>();
            var skillNode = vocNode.SelectSingleNode("skill");
            if (skillNode != null)
            {
                skills[SkillType.Fist] = float.Parse(skillNode.Attributes["fist"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Club] = float.Parse(skillNode.Attributes["club"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Sword] = float.Parse(skillNode.Attributes["sword"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Axe] = float.Parse(skillNode.Attributes["axe"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Distance] = float.Parse(skillNode.Attributes["distance"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Shielding] = float.Parse(skillNode.Attributes["shielding"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Fishing] = float.Parse(skillNode.Attributes["fishing"].Value, CultureInfo.InvariantCulture);
                skills[SkillType.Magic] = 1.0f; // padrão fixo
            }

            vocation.Skills = skills;
            vocations.Add(vocation);
        }
        
        return vocations;
    }
}
