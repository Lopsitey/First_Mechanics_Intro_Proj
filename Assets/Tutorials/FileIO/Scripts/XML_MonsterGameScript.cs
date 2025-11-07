using System;
using TMPro;
using UnityEngine;

public class XML_MonsterGameScript : MonoBehaviour
{
    [SerializeField] private TMP_Text m_MonsterCount;
    [SerializeField] private TMP_InputField m_MonsterName;
    [SerializeField] private TMP_InputField m_MonsterHealth;


    [SerializeField] private XML_MonsterContainer m_MonsterContainerRef;
    private XML_Monster m_MonsterRef;

    private void Start()
    {
        m_MonsterContainerRef = new XML_MonsterContainer();

        UpdateUI();
    }

    public void AddMonster()
    {
        m_MonsterRef = new XML_Monster();

        m_MonsterRef.Name = m_MonsterName.text;
        if (Int32.TryParse(m_MonsterHealth.text, out int health))
            m_MonsterRef.Health = health;
        else
            m_MonsterRef.Health = 100;

        m_MonsterContainerRef.MonsterContainer.Add(m_MonsterRef);

        UpdateUI();
    }
    public void RemoveMonster()
    {
        for (int i = 0; i < m_MonsterContainerRef.MonsterContainer.Count; i++)
        {
            if (m_MonsterContainerRef.MonsterContainer[i].Name == m_MonsterName.text)
            {
                m_MonsterContainerRef.MonsterContainer.RemoveAt(i);
                break;
            }
        }

        UpdateUI();
    }

    public void SaveData()
    {
        XML_SaveAndLoadExample.SaveXML(m_MonsterContainerRef);

        UpdateUI();
    }

    public void LoadData()
    {
        XML_MonsterContainer TempMonsterContainer = XML_SaveAndLoadExample.LoadXML();

        m_MonsterContainerRef.MonsterContainer = TempMonsterContainer.MonsterContainer;

        UpdateUI();
    }

    void UpdateUI()
    {
        m_MonsterCount.text = $"Monsters = {m_MonsterContainerRef.MonsterContainer.Count}";
    }
}
