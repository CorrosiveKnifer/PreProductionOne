using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFiller : MonoBehaviour
{
    public void FillWater()
    {
        GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>()?.AddItem(ItemObject.CreateItem(7, 25));
    }
}
