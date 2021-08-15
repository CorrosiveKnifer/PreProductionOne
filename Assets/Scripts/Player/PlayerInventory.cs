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
    
    
    public void Start()
    {
        int columns = GameManager.instance.m_saveSlot.GetPlayerIntegerData("backpack_column");
        int rows = GameManager.instance.m_saveSlot.GetPlayerIntegerData("backpack_row");
        int hotbarCount = GameManager.instance.m_saveSlot.GetPlayerIntegerData("hotbar_column");

        m_itemGrid = new ItemObject[columns, rows];
        m_hotbarItem = new ItemObject[5,1];

        m_size = new Vector2Int(columns, rows);

        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                m_itemGrid[c, r] = GameManager.instance.m_saveSlot.GetPlayerBackpackData(c, r);
                if(m_itemGrid[c, r] != null)
                {
                    m_itemGrid[c, r].m_amount = (uint)Mathf.Clamp(m_itemGrid[c, r].m_amount, 0, ItemObject.MAX_AMOUNT);
                }
            }
        }
        for (int c = 0; c < hotbarCount; c++)
        {
            m_hotbarItem[c, 0] = GameManager.instance.m_saveSlot.GetPlayerHotbarData(c);
        }

        AddItem(ItemObject.CreateItem(6, 50));
        AddItem(ItemObject.CreateItem(5, 1));
        AddItem(ItemObject.CreateItem(0, 5));
        AddItem(ItemObject.CreateItem(1, 5));
        AddItem(ItemObject.CreateItem(3, 5));
        AddItem(ItemObject.CreateItem(4, 5));

        m_hotbar.Generate(m_hotbarItem, new Vector2Int(5, 1));
    }

    public void RemoveItem(int m_itemId, int m_amount)
    {
        if (m_itemId == -1 || m_amount == 0)
            return;

        for (int c = 0; c < m_size.x; c++)
        {
            for (int r = 0; r < m_size.y; r++)
            {
                if(m_itemGrid[c, r] != null && m_itemGrid[c, r].m_id == m_itemId)
                {
                    if (m_itemGrid[c, r].m_amount < m_amount)
                    {
                        m_amount -= (int)m_itemGrid[c, r].m_amount;
                        m_itemGrid[c, r] = null;
                    }
                    else if (m_itemGrid[c, r].m_amount > m_amount)
                    {
                        m_itemGrid[c, r].m_amount -= (uint)m_amount;
                        return;
                    }
                    else
                    {
                        m_itemGrid[c, r] = null;
                        return;
                    }
                }
            }
        }
        for (int c = 0; c < m_hotbarItem.GetLength(0); c++)
        {
            if (m_hotbarItem[c, 0] != null && m_hotbarItem[c, 0].m_id == m_itemId)
            {
                if (m_hotbarItem[c, 0].m_amount < m_amount)
                {
                    m_amount -= (int)m_hotbarItem[c, 0].m_amount;
                    m_hotbarItem[c, 0] = null;
                }
                else if (m_hotbarItem[c, 0].m_amount > m_amount)
                {
                    m_hotbarItem[c, 0].m_amount -= (uint)m_amount;
                    return;
                }
                else
                {
                    m_hotbarItem[c, 0] = null;
                    return;
                }
            }
        }
    }

    public bool ContainsItem(int m_itemId, int m_amount)
    {
        foreach (var item in m_itemGrid)
        {
            if(item != null && item.m_id == m_itemId)
            {
                m_amount -= (int)item.m_amount;

                if (m_amount <= 0)
                    return true;
            }
        }

        foreach (var item in m_hotbarItem)
        {
            if (item != null && item.m_id == m_itemId)
            {
                m_amount -= (int)item.m_amount;

                if (m_amount <= 0)
                    return true;
            }
        }

        return false;
    }

    public ItemObject GetSelectItem()
    {
        return m_hotbar.GetSelectItem();
    }

    private void Update()
    {
        if(m_display.m_hasUpdated)
        {
            m_display.UpdateInventory(m_itemGrid);
        }
        if (m_hotbar.m_hasUpdated)
        {
            m_hotbar.UpdateInventory(m_hotbarItem);
        }
    }

    public void GenerateOnDisplay(bool isActive)
    {
        m_display.Generate(m_itemGrid, m_size);
        m_display.gameObject.SetActive(isActive);
    }

    public void OnDestroy()
    {
        if(GameManager.HasInstance())
        {
            SaveToSlot(GameManager.instance.m_saveSlot);
        }
    }

    public void SaveToSlot(SaveSlot slot)
    {
        if(m_itemGrid != null)
        {
            for (int c = 0; c < m_size.x; c++)
            {
                for (int r = 0; r < m_size.y; r++)
                {
                    if (m_itemGrid[c, r] != null)
                        slot.SavePlayerBackpackData(c, r, m_itemGrid[c, r].m_id, (uint)m_itemGrid[c, r].m_amount);
                    else
                        slot.SavePlayerBackpackData(c, r, -1, 0);
                }
            }
        }

        if (m_itemGrid != null)
        {
            for (int c = 0; c < m_hotbarItem.GetLength(0); c++)
            {
                if (m_hotbarItem[c, 0] != null)
                    slot.SavePlayerHotbarData(c, m_hotbarItem[c, 0].m_id, (uint)m_hotbarItem[c, 0].m_amount);
                else
                    slot.SavePlayerHotbarData(c, -1, 0);

            }
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
                m_hotbar.SetSlotItem(null, index, 0);
                m_hotbarItem[index, 0] = null;
            }
        }
    }

    public void SelectItem(int index)
    {
        m_hotbar.SelectItem(index, 0);
    }

    public void AddItem(ItemObject addition)
    {
        ItemObject nullSlot = null;
        ref ItemObject slot = ref nullSlot;
        bool foundDupe = false;

        for (int c = 0; c < m_size.x; c++)
        {
            if (foundDupe)
                break;

            for (int r = 0; r < m_size.y; r++)
            {
                if (m_itemGrid[c, r] == null || m_itemGrid[c, r].m_id == -1)
                {
                    slot = ref m_itemGrid[c, r];
                    continue;
                }
                if (m_itemGrid[c, r].m_id == addition.m_id && m_itemGrid[c, r].m_amount != ItemObject.MAX_AMOUNT)
                {
                    slot = ref m_itemGrid[c, r];
                    foundDupe = true;
                    break;
                }
            }
        }

        if (!foundDupe)
        {
            for (int c = m_hotbarItem.Length - 1; c >= 0; c--)
            {
                if (m_hotbarItem[c, 0] == null || m_hotbarItem[c, 0].m_id == -1)
                {
                    slot = ref m_hotbarItem[c, 0];
                    continue;
                }
                if (m_hotbarItem[c, 0].m_id == addition.m_id && m_hotbarItem[c, 0].m_amount != ItemObject.MAX_AMOUNT)
                {
                    slot = ref m_hotbarItem[c, 0];
                    foundDupe = true;
                    break;
                }
            }
        }

        if (slot != nullSlot && slot.m_id == addition.m_id)
        {
            if(addition.m_type != ItemType.Tool)
            {
                if(addition.m_amount + slot.m_amount > ItemObject.MAX_AMOUNT)
                {
                    uint diff = (addition.m_amount + slot.m_amount) - (uint)ItemObject.MAX_AMOUNT;
                    slot.m_amount = (uint)ItemObject.MAX_AMOUNT;
                    AddItem(ItemObject.CreateItem(addition.m_id, (uint)diff));
                }
                else
                {
                    slot.m_amount += addition.m_amount;
                }
            }
            else if(addition.GetToolType() == ToolType.WaterCan)
            {
                slot.m_amount = (uint)Mathf.Clamp(addition.m_amount + slot.m_amount, 0, ItemObject.MAX_AMOUNT);
            }
        }
        else
        {
            if (addition.m_type != ItemType.Tool || !this.ContainsItem(addition.m_id, 1))
            {
                slot = addition;
            }            
        }

        UpdateDisplay();
        return;
    }

    public void UpdateDisplay()
    {
        if(m_display.isActiveAndEnabled)
        {
            for (int c = 0; c < m_itemGrid.GetLength(0); c++)
            {
                for (int r = 0; r < m_itemGrid.GetLength(1); r++)
                {
                    m_display.SetSlotItem(m_itemGrid[c, r], c, r);
                }
            }
        }
        
        for (int c = 0; c < m_hotbarItem.GetLength(0); c++)
        {
            m_hotbar.SetSlotItem(m_hotbarItem[c, 0], c, 0);
        }
    }
}
