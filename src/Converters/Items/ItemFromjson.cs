using Converters.Items;
using System.Collections.Generic;
using System.Xml;

public class ItemFromjson
{
    public List<ItemOutput> Convert(XmlDocument doc)
    {
        var items = new List<ItemOutput>();
        var itemNodes = doc.SelectNodes("/items/item");

        foreach (XmlNode node in itemNodes)
        {
            var item = new ItemOutput
            {
                Id = node.Attributes["id"]?.Value,
                FromId = node.Attributes["fromid"]?.Value,
                ToId = node.Attributes["toid"]?.Value,
                Name = node.Attributes["name"]?.Value,
                Plural = node.Attributes["plural"]?.Value,
                Article = node.Attributes["article"]?.Value
            };

            var attributeNodes = node.SelectNodes("attribute");
            if (attributeNodes.Count > 0)
            {
                var attrList = new List<ItemAttributeOutput>();
                foreach (XmlNode attr in attributeNodes)
                {
                    var key = attr.Attributes["key"]?.Value;
                    var value = attr.Attributes["value"]?.Value;
                    if (!string.IsNullOrEmpty(key) && value != null)
                    {
                        attrList.Add(new ItemAttributeOutput
                        {
                            Key = key,
                            Value = value
                        });
                    }
                }

                if (attrList.Count > 0)
                    item.Attributes = attrList;
            }

            items.Add(item);
        }

        return items;
    }
}
