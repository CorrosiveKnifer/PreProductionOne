using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestItem : UI_Element
{
    public Quest m_quest;
    [Header("Dependencies")]
    public bool m_isShowingDependencies = true;

    [ShowIf("m_isShowingDependencies")]
    public Image m_itemIcon;
    [ShowIf("m_isShowingDependencies")]
    public Text m_itemAmount;
    [ShowIf("m_isShowingDependencies")]
    public Text m_remainingText;


    // Start is called before the first frame update
    void Start()
    {
        SetQuest(m_quest);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_quest.GetRemainingDays() == 0)
        {
            m_remainingText.fontStyle = FontStyle.Bold;
            m_remainingText.text = $"Due today!";
        }
        else
        {
            m_remainingText.text = $"Days remaining: {m_quest.GetRemainingDays()}";
            m_remainingText.fontStyle = FontStyle.Normal;
        }
    }

    public void SetQuest(Quest _quest)
    {
        m_quest = _quest;

        if (m_quest == null)
        {
            return;
        }

        string name = GameManager.instance.m_items.list[m_quest.m_itemId].inventoryImageName;
        m_itemIcon.sprite = Resources.Load<Sprite>(name);
        m_itemAmount.text = m_quest.m_amount.ToString();
        m_remainingText.text = $"Days remaining: {m_quest.GetRemainingDays()}";
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        //Do Nothing
        return false;
    }

    public override void OnMouseDownEvent()
    {
        //Do Nothing
    }

    public override void OnMouseUpEvent()
    {
        //Do Nothing
    }
}
