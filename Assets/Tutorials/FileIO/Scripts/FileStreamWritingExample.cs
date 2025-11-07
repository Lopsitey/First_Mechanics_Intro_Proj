using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileStreamWriting : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_InputField;


    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public void SaveData()
    {
        StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + "ExampleInput.txt");
        streamWriter.Write(m_InputField.text);
        streamWriter.Close();
    }

    public void LoadData()
    {
        StreamReader streamReader = new StreamReader(Application.persistentDataPath + "ExampleInput.txt");
        m_InputField.text = streamReader.ReadToEnd();
        streamReader.Close();
    }

    public void ResetInput()
    {
        m_InputField.text = "";
    }
}
