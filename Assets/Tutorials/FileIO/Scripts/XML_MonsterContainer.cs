using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class XML_MonsterContainer 
{
    [XmlArray("Monsters")]
    [XmlArrayItem("Monster")]
    public List<XML_Monster> MonsterContainer = new List<XML_Monster>();
}
