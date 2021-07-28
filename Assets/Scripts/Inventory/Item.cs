using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Item
{
    public enum ItemType
    {
        Null,
        Crop,
    }

    public ItemType itemType;
    public int amount;
}
