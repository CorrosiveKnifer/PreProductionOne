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
    [SerializeField] private UI_GridDisplay m_display;

    private Vector2Int m_size;
    private float m_inputDelay;
    public void Start()
    {
        m_itemGrid = new ItemObject[m_display.m_columnCount, m_display.m_rowCount];
        m_size = new Vector2Int(m_display.m_columnCount, m_display.m_rowCount);

        for (int c = 0; c < m_display.m_columnCount; c++)
        {
            for (int r = 0; r < m_display.m_rowCount; r++)
            {
                m_itemGrid[c, r] = null;
            }
        }

        ItemElement itemDef;
        if(GameManager.instance.m_items.dictionary.TryGetValue("Corn", out itemDef))
        {
            m_itemGrid[0, 0] = new ItemObject(itemDef, 1);
        }
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
    }
}
