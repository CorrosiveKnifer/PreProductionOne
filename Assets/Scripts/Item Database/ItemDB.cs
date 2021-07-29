using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

 
public class ItemDB
{
    [Serializable]
    class Items
    {
        public ItemElement[] list;
    }

    public Dictionary<string, ItemElement> dictionary;
    public ItemDB(string jsonData)
    {
        Items itemList = JsonUtility.FromJson<Items>(jsonData);
        dictionary = new Dictionary<string, ItemElement>();
        foreach (var item in itemList.list)
        {
            dictionary.Add(item.name, item);
            item.UpdateElement();
        }

    }
}
