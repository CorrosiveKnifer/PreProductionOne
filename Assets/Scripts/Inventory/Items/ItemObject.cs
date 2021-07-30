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
public class ItemObject
{
    public string name;
    public string groundPrefabName;
    public string inventoryImageName;
    public string itemType;

    protected GameObject groundPrefab;
    protected Sprite inventoryImage;

}
