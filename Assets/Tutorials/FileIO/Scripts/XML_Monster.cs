using System.Xml.Serialization;

[System.Serializable]
public class XML_Monster 
{
    [XmlAttribute("name")]
    public string Name;

    public int Health;
}
