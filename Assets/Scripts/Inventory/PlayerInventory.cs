using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Slot
{
    public Slot(Vector2 slotLoc) 
    { 
        item = null; amount = 0; location = slotLoc; 
    }

    public ItemObject item;
    public int amount;
    public Vector2 location;
}

class PlayerInventory : MonoBehaviour
{
    private ItemObject[,] m_itemGrid;
    private ItemObject[,] m_hotbarItem;
    //private 
    [SerializeField] private UI_GridDisplay m_display;
    [SerializeField] private UI_GridDisplay m_hotbar;
    private Vector2Int m_size;
    private float m_inputDelay;

    public void Start()
    {
        m_itemGrid = new ItemObject[m_display.m_columnCount, m_display.m_rowCount];
        m_hotbarItem = new ItemObject[5,1];

        m_size = new Vector2Int(m_display.m_columnCount, m_display.m_rowCount);

        for (int c = 0; c < m_display.m_columnCount; c++)
        {
            for (int r = 0; r < m_display.m_rowCount; r++)
            {
                m_itemGrid[c, r] = null;
            }
        }
        for (int c = 0; c < 5; c++)
        {
            m_hotbarItem[c, 0] = null;
        }

        ItemElement itemDef;
        if(GameManager.instance.m_items.dictionary.TryGetValue("Corn", out itemDef))
        {
            m_itemGrid[0, 0] = ScriptableObject.CreateInstance("ItemObject") as ItemObject;
            m_itemGrid[0, 0].m_definition = itemDef;
            m_itemGrid[0, 0].m_amount = 1;

            m_hotbarItem[0, 0] = ScriptableObject.CreateInstance("ItemObject") as ItemObject;
            m_hotbarItem[0, 0].m_definition = itemDef;
            m_hotbarItem[0, 0].m_amount = 1;
        }

        m_hotbar.Generate(m_hotbarItem, new Vector2Int(5, 1));
    }

    private void Update()
    {
        if(m_inputDelay > 0.0f)
        {
            m_inputDelay -= Time.deltaTime;
            return;
        }

        if(InputManager.instance.IsKeyDown(KeyType.I))
        {
            m_inputDelay = 0.25f;
            if (!m_display.enabled)
            {
                m_display.Generate(m_itemGrid, m_size);
            }
            else
            {
                m_display.UpdateInventory(m_itemGrid);
            }

            m_display.enabled = !m_display.enabled;
        }

        if(m_display.m_hasUpdated)
        {
            m_display.UpdateInventory(m_itemGrid);
        }
        if (m_hotbar.m_hasUpdated)
        {
            m_hotbar.UpdateInventory(m_hotbarItem);
        }
    }

    public ItemObject GetItemFromHotbar(int index)
    {
        return m_hotbarItem[index, 0];
    }

    public void UseItem(int index)
    {
        if(m_hotbarItem[index, 0] != null)
        {
            m_hotbarItem[index, 0].m_amount--;
            if(m_hotbarItem[index, 0].m_amount == 0)
            {
                m_hotbarItem[index, 0] = null;
            }
        }
    }
}
