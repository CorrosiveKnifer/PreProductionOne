using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : ScriptableObject
{
    public ItemElement m_definition = null;
    public uint m_amount = 0;

    public ItemObject(ItemElement myType, uint amount)
    {
        m_definition = myType;
        m_amount = amount;
    }
}
