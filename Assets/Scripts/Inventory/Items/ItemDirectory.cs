using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemDirectory
{
    [Serializable]
    class Items
    {
        public ItemObject[] list;
    }

    public Dictionary<string, ItemObject> dictionary;
    public ItemDirectory(string jsonData)
    {
        Items itemList = JsonUtility.FromJson<Items>(jsonData);
        dictionary = new Dictionary<string, ItemObject>();
        foreach (var item in itemList.list)
        {
            dictionary.Add(item.name, item);
        }

    }
}
