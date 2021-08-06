using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public void GiveQuestToPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuests>().AddQuest(new Quest(Random.Range(0, 3), 5, 7));
    }

}
