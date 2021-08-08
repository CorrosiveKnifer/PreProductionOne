using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Crop,
    Tool,
    CropUtil,
};
public enum ToolType
{
    Null,
    Hoe,
    Shovel,
    WaterCan,
}

public class ItemObject : ScriptableObject
{
    public int m_id = -1;
    public string m_placePrefabName;
    public string m_inventoryImageName;
    public string m_dropPrefabName;
    public ItemType m_type;

    public uint m_amount = 0;

    public static ItemObject CreateItem(int index, uint amount)
    {
        if (index < 0 || amount == 0)
            return null;

        ItemObject temp = ScriptableObject.CreateInstance("ItemObject") as ItemObject;

        temp.m_id = index;
        temp.m_placePrefabName = GameManager.instance.m_items.list[index].placePrefabName;
        temp.m_inventoryImageName = GameManager.instance.m_items.list[index].inventoryImageName;
        temp.m_dropPrefabName = GameManager.instance.m_items.list[index].dropPrefabName;

        switch (GameManager.instance.m_items.list[index].itemType.ToLower())
        {
            case "crop":
                temp.m_type = ItemType.Crop;
                break;
            case "tool":
                temp.m_type = ItemType.Tool;
                break;
            case "croputil":
                temp.m_type = ItemType.CropUtil;
                break;
            default:
                break;
        }
        temp.m_amount = amount;

        return temp;
    }

    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>(m_inventoryImageName);
    }

    public GameObject GetPlaceObject()
    {
        return Resources.Load<GameObject>(m_placePrefabName);
    }

    public GameObject GetDropObject()
    {
        return Resources.Load<GameObject>(m_dropPrefabName);
    }

    public ToolType GetToolType()
    {
        switch (GameManager.instance.m_items.list[m_id].name.ToLower())
        {
            case "hoe":
                return ToolType.Hoe;
            case "shovel":
                return ToolType.Shovel;
            case "Watering Can":
                return ToolType.WaterCan;
            default:
                return ToolType.Null;
        }
    }
}
