#region

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#endregion

public static class BinarySaveAndLoad
{
    //this class uses a BinaryFormatter to serialize game data to a binary file
    public static void
        BinarySave(BF_GameExample GameObj)
    {
        //takes the class that holds the data to be saved as a parameter
        BinaryFormatter bf = new BinaryFormatter(); //new BinaryFormatter instance

        //creates or overwrites a file named SaveData.sav using the persistent data path
        FileStream file = new FileStream(Application.persistentDataPath + "/SaveData.sav", FileMode.Create);

        //creates a new instance of the game object that uses the binary formatter and passes in the data holding class as a param
        BF_GameDataExample
            gameData = new BF_GameDataExample(GameObj); //it needs the game object because it extracts the data from it

        //outputs the file path to the console for debugging purposes
        Debug.Log(Application.persistentDataPath + "/SaveData.sav");

        //serializes the game data and writes it to the file
        bf.Serialize(file, gameData);

        //closes the file stream to free up resources
        file.Close();
    }

    public static BF_GameDataExample BinaryLoad()
    {
        //if the save file does not exist, return null
        if (!File.Exists(Application.persistentDataPath + "/SaveData.sav"))
        {
            return null;
        }

        //new BinaryFormatter instance
        BinaryFormatter bf = new BinaryFormatter();

        //opens the existing save file using the persistent data path
        FileStream file = new FileStream(Application.persistentDataPath + "/SaveData.sav", FileMode.Open);

        //deserializes the file content back into a BF_GameDataExample object
        BF_GameDataExample gameData = (BF_GameDataExample)bf.Deserialize(file);

        //closes the file stream to free up resources
        file.Close();
        //returns the deserialized game data - good for checking if the load was successful
        return gameData;
    }
}