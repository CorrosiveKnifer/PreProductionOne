using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenInventoryLink : MonoBehaviour
{
    public GameObject DialogBox;
    public void OpenInventory()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OpenInventory();
    }
    public void OpenQuests()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OpenQuests();
    }
    private void Update()
    {
        if(DialogBox != null)
            GetComponent<Button>().interactable = !DialogBox.activeInHierarchy;
    }
    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
