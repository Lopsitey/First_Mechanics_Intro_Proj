using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class XML_SaveAndLoadExample
{
    public static void SaveXML(XML_MonsterContainer GameObj)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XML_MonsterContainer));

        FileStream file = new FileStream(Application.persistentDataPath + "/SaveDataXML.XML", FileMode.Create);

        serializer.Serialize(file, GameObj);

        Debug.Log(Application.persistentDataPath + "/SaveDataXML.XML");

        file.Close();
    }

    public static XML_MonsterContainer LoadXML()
    {
        if (!File.Exists(Application.persistentDataPath + "/SaveDataXML.XML")) { return null; }

        XmlSerializer serializer = new XmlSerializer(typeof(XML_MonsterContainer));

        FileStream file = new FileStream(Application.persistentDataPath + "/SaveDataXML.XML", FileMode.Open);

        XML_MonsterContainer monsterContainer = serializer.Deserialize(file) as XML_MonsterContainer;

        file.Close();
        return monsterContainer;
    }
}
