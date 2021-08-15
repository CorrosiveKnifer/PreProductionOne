using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFiller : MonoBehaviour
{
    public void Interact()
    {
        ItemObject item = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().GetSelectItem();
        if (item?.GetToolType() == ToolType.WaterCan)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().StartWaterActionOn(gameObject);
            GetComponent<SoloAudioAgent>().Play();
            return;
        }
    }

    public void FillWater()
    {
        GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>()?.AddItem(ItemObject.CreateItem(6, 25));
    }
}
