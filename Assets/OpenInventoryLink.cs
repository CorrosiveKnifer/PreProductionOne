using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInventoryLink : MonoBehaviour
{
    public void OpenInventory()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OpenInventory();
    }
    public void OpenQuests()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OpenQuests();
    }
    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
