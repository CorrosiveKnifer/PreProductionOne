using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ItemType
{
    Null,
    Crop,
}

[Serializable]
public class ItemElement
{
    public string name;
    public string groundPrefabName;
    public string inventoryImageName;
    public string itemType;

    public GameObject groundPrefab { get; protected set; }
    public Sprite inventoryImage { get; protected set; }

    public void UpdateElement()
    {
        inventoryImage = Resources.Load<Sprite>(inventoryImageName);
        groundPrefab = Resources.Load<GameObject>(groundPrefabName);
    }
}
