using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public void GiveQuestToPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuests>().AddQuest(new Quest(0, 5, 7));
    }

}
