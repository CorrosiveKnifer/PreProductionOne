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
    public int m_lastDayChecked;

    // Start is called before the first frame update
    void Start()
    {
        m_playerQuests = new List<Quest>();
        foreach (var item in GameManager.instance.m_saveSlot.GetQuests())
        {
            m_playerQuests.Add(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        List<Quest> toRemove = null;
        foreach (var quest in m_playerQuests)
        {
            if(quest.m_dueDay == GameManager.instance.m_day)
            {
                if(toRemove == null)
                {
                    toRemove = new List<Quest>();
                }
                toRemove.Add(quest);
            }
        }

        if (toRemove != null)
        {
            foreach (var item in toRemove)
            {
                GameManager.instance.m_saveSlot.RemoveQuest(item);
                m_playerQuests.Remove(item);
                GameManager.instance.m_questsFailed += 1;
            }

            if (m_display.gameObject.activeInHierarchy)
            {
                m_display.ClearList();
                GenerateOnDisplay(m_display.gameObject.activeInHierarchy);
            }
        }
    }

    public void GenerateOnDisplay(bool isActive)
    {
        m_display.gameObject.SetActive(isActive);
        m_display.Generate(m_playerQuests);
    }
    public void ClearDisplay()
    {
        m_display.ClearList();
    }

    public void AddQuest(Quest quest)
    {
        m_playerQuests.Add(quest);
        GameManager.instance.m_saveSlot.AddQuest(quest);
        m_display.ShowNewQuestDisplay();
        HUDManager.instance.GetComponent<MultiAudioAgent>().PlayOnce("NewQuest");
        if (m_display.gameObject.activeInHierarchy)
        {
            m_display.ClearList();
            GenerateOnDisplay(m_display.gameObject.activeInHierarchy);
        }
    }

    public int RedeemQuests()
    {
        int result = 0;

        if (m_playerQuests.Count == 0)
            return -1;

        List<Quest> toRemove = new List<Quest>();

        foreach (var item in m_playerQuests)
        {
            if(GetComponent<PlayerInventory>().ContainsItem(item.m_itemId, item.m_amount))
            {
                GetComponent<PlayerInventory>().RemoveItem(item.m_itemId, item.m_amount);
                toRemove.Add(item);
            }
        }

        result = toRemove.Count;
        foreach (var item in toRemove)
        {
            GameManager.instance.m_saveSlot.RemoveQuest(item);
            m_playerQuests.Remove(item);
        }
        GameManager.instance.m_questsDone += result;

        if (m_display.gameObject.activeInHierarchy)
        {
            m_display.ClearList();
            GenerateOnDisplay(m_display.gameObject.activeInHierarchy);
        }
        return result;
    }
}
