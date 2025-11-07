using System;
using UnityEngine;

[System.Serializable]
public class BF_GameDataExample 
{//this is the class that holds the data to be saved
    public int score;
    public int lives;
    public int level;
    public float[] position = new float[3];

    public BF_GameDataExample(BF_GameExample data) 
    {
        score = data.m_Score;
        lives = data.m_Lives;
        level = data.m_Level;
        position[0] = data.m_position.x;
        position[1] = data.m_position.y;
        position[2] = data.m_position.z;
    }
}
