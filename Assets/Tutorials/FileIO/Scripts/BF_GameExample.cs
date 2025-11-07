#region

using TMPro;
using UnityEngine;

#endregion

public class BF_GameExample : MonoBehaviour
{//this is the script for the object that uses the game save data
    
    //all of these fields are used for displaying th GUI data 
    [SerializeField] public int m_Score;
    [SerializeField] public int m_Lives;
    [SerializeField] public int m_Level;

    [SerializeField] public Vector3 m_position;

    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private TMP_Text m_LivesText;
    [SerializeField] private TMP_Text m_LevelText;
    [SerializeField] private TMP_Text m_PositionText;

    // The OnValidate method is an editor-only method that triggers when a value is updated in the inspector, or when the editor updates.
    // https://docs.unity3d.com/6000.2/Documentation/ScriptReference/MonoBehaviour.OnValidate.html
    private void OnValidate()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_ScoreText.text = $"Score = {m_Score}";
        m_LivesText.text = $"Lives = {m_Lives}";
        m_LevelText.text = $"Level = {m_Level}";
        m_PositionText.text = $"Position = {m_position}";
    }

    public void SaveData()//bound through a GUI on click event on the canvas button
    {
        BinarySaveAndLoad.BinarySave(this);
    }

    public void LoadData()
    {
        BF_GameDataExample gameData = BinarySaveAndLoad.BinaryLoad();

        m_Score = gameData.score;
        m_Lives = gameData.lives;
        m_Level = gameData.level;
        m_position.x = gameData.position[0];
        m_position.y = gameData.position[1];
        m_position.z = gameData.position[2];

        UpdateUI();
    }

    public void ClearData()
    {
        m_Score = 0;
        m_Lives = 0;
        m_Level = 0;
        m_position = Vector3.zero;

        UpdateUI();
    }
}