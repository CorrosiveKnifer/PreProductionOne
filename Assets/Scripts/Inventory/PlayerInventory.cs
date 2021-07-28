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
    private Slot[,] m_itemGrid;
    [SerializeField] private InventoryDisplay m_display;

    public void Start()
    {
        m_itemGrid = new Slot[m_display.m_columnCount, m_display.m_rowCount];
        for (int c = 0; c < m_display.m_columnCount; c++)
        {
            for (int r = 0; r < m_display.m_rowCount; r++)
            {
                m_itemGrid[c, r] = new Slot(new Vector2(c, r));
            }
        }

        GameManager.instance.m_items.dictionary.TryGetValue("Corn", out m_itemGrid[0, 0].item);
    }

    private void Update()
    {
        for (int c = 0; c < m_display.m_columnCount; c++)
        {
            for (int r = 0; r < m_display.m_rowCount; r++)
            {
                if(m_itemGrid[c, r].item != null)
                {
                    //Note: add error message:
                    m_display.SetSpriteInCell(c, r, Resources.Load<Sprite>(m_itemGrid[c, r].item.inventoryImageName));
                }
                else
                {
                    m_display.SetSpriteInCell(c, r, null);
                }
            }
        }
    }
}
