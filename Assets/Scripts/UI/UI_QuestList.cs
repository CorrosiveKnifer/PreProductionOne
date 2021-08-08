using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestList: UI_Element
{
    public GameObject m_contentParent;

    [SerializeField] private GameObject m_item;
    public List<UI_QuestItem> m_list;

    // Start is called before the first frame update
    void Start()
    {
        m_list = new List<UI_QuestItem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        ClearList();
    }

    public void Generate(List<Quest> _quests)
    {
        foreach (var item in _quests)
        {
            m_list.Add(Instantiate(m_item, m_contentParent.transform).GetComponent<UI_QuestItem>());
            m_list[m_list.Count - 1].SetQuest(item);
        }
    }

    public void ClearList()
    {
        foreach (var item in m_list)
        {
            Destroy(item.gameObject);
        }
        m_list = new List<UI_QuestItem>();
    }


    public override bool IsContainingVector(Vector2 _pos)
    {
        //Do nothing
        return false;
    }

    public override void OnMouseDownEvent()
    {
        //Do nothing
    }

    public override void OnMouseUpEvent()
    {
        //Do nothing
    }
}
