using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestList: UI_Element
{
    public GameObject m_contentParent;

    [SerializeField] private GameObject m_item;
    [SerializeField] private List<UI_QuestItem> m_list;
    [SerializeField] private Text m_noQuestText;
    [SerializeField] private GameObject m_newQuest;

    // Start is called before the first frame update
    void Start()
    {
        m_list = new List<UI_QuestItem>();
    }

    // Update is called once per frame
    void Update()
    {
        m_noQuestText.gameObject.SetActive(m_contentParent.GetComponentsInChildren<UI_QuestItem>().Length == 0);
    }

    public void Generate(List<Quest> _quests)
    {
        if(m_contentParent.GetComponentsInChildren<UI_QuestItem>().Length != 0)
        {
            ClearList();
        }

        foreach (var item in _quests)
        {
            m_list.Add(Instantiate(m_item, m_contentParent.transform).GetComponent<UI_QuestItem>());
            m_list[m_list.Count - 1].SetQuest(item);
        }
    }
    public void OnEnable()
    {
        m_newQuest.SetActive(false);
    }

    public void ShowNewQuestDisplay()
    {
        m_newQuest.SetActive(true);
    }

    public void ClearList()
    {
        UI_QuestItem[] _list = m_contentParent.GetComponentsInChildren<UI_QuestItem>();
        for (int i = 0; i < _list.Length; i++)
        {
            Destroy(_list[i].gameObject);
        }
        m_list.Clear();
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
