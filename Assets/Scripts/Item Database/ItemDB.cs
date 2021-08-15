using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ItemDB
{
    [Serializable]
    public class ItemElement
    {
        public int id;
        public string name;
        public string pluralName;
        public string placePrefabName;
        public string inventoryImageName;
        public string dropPrefabName;
        public string itemType;

    }

    public ItemElement[] list;
}
