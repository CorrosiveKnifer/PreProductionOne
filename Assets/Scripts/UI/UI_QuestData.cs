using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestData : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<Text>().text = $"Quests: {GameManager.instance.m_questsDone} - {GameManager.instance.m_questsFailed}";
    }
}
