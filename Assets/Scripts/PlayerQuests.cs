using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Quest
{
    public int m_itemId = -1;
    public int m_amount = 0;
    public int m_dueDay = 0;

    public Quest(int id, int amount, int due)
    {
        m_itemId = id;
        m_amount = amount;
        m_dueDay = due;
    }

    public int GetRemainingDays()
    {
        return m_dueDay - GameManager.instance.m_day;
    }
}

public class PlayerQuests : MonoBehaviour
{
    [SerializeField] private UI_QuestList m_display;

    public List<Quest> m_playerQuests;

    public void AddQuest(Quest quest)
    {
        m_playerQuests.Add(quest);
        Debug.Log("Quest Added.");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerQuests = new List<Quest>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateOnDisplay(bool isActive)
    {
        m_display.Generate(m_playerQuests);
        m_display.gameObject.SetActive(isActive);
    }
}
