using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * UI_SlotDisplay by Michael Jordan
 * File: UI_SlotDisplay.cs
 * Description:
 *		Used to manage the individual slot within a grid of slots.
 */
public class UI_GridDisplay : UI_Element
{
    public bool generateOnAwake = false;

    public Vector2Int m_spacing;
    public bool m_hasUpdated = false;

    [Range(1, 20)]
    public int m_rowCount = 5;
    [Range(1, 20)]
    public int m_columnCount = 5;
    [Range(0.0f, 1.0f)]
    public float m_itemScale = 1.0f;

    public GameObject m_buttonPrefab;

    private UI_SlotDisplay[,] m_grid; //[x, y]
    private UI_SlotDisplay m_heldItem = null;
    private UI_SlotDisplay m_selectSlot = null;

    // Start is called before the first frame update
    void Awake()
    {
        if(generateOnAwake)
        {
            Generate(null, new Vector2Int(m_columnCount, m_rowCount));
        }

        //~~~ Start creating the layout ~~~
        GridLayoutGroup layout = gameObject.AddComponent<GridLayoutGroup>();

        //Current width and height of the grid:
        Vector2 fullSize = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);

        //Width/height without the padding space:
        Vector2 useableSize = new Vector2(fullSize.x - (m_columnCount + 1 ) * m_spacing.x, fullSize.y - (m_rowCount + 1) * m_spacing.y);

        //Cell size:
        layout.cellSize = new Vector2(useableSize.x / m_columnCount, useableSize.y / m_rowCount);

        //Padding:
        layout.spacing = m_spacing;
        layout.padding = new RectOffset(m_spacing.x, m_spacing.x, m_spacing.y, m_spacing.y);
        //~~~ End creating the layout ~~~
    }

    // Update is called once per frame
    void Update()
    {
        if(m_heldItem != null)
        {
            m_heldItem.MoveImageTo(InputManager.instance.GetMousePositionInScreen());
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    public void Generate(ItemObject[,] _itemGrid, Vector2Int _size)
    {
        if(m_grid != null)
        {
            return;
        }

        if (_itemGrid == null)
        {
            GenerateCleared();
            return;
        }
            
        m_columnCount = _size.x;
        m_rowCount = _size.y;
        m_grid = new UI_SlotDisplay[m_columnCount, m_rowCount];
        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                m_grid[c, r] = GameObject.Instantiate(m_buttonPrefab, transform).GetComponent<UI_SlotDisplay>();
                m_grid[c, r].gameObject.name = $"Slot[{c},{r}]";
                m_grid[c, r].SetItem(_itemGrid[c, r]);
            }
        }
    }

    public void SetSlotItem(ItemObject _itemGrid, int column, int row)
    {
        if (m_grid == null)
            return;

        m_grid[column, row].SetItem(_itemGrid);
    }

    private void GenerateCleared()
    {
        m_grid = new UI_SlotDisplay[m_columnCount, m_rowCount];

        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                m_grid[c, r] = GameObject.Instantiate(m_buttonPrefab, transform).GetComponent<UI_SlotDisplay>();
                m_grid[c, r].gameObject.name = $"Slot[{c},{r}]";
            }
        }
    }

    public void UpdateInventory(ItemObject[,] _itemGrid)
    {
        if (m_grid == null)
            return;

        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                _itemGrid[c, r] = m_grid[c, r].GetItem();
            }
        }
        m_hasUpdated = false;
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        if (m_grid == null)
            return false;

        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                if (m_grid[c, r].IsContainingVector(_pos))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public ItemObject GetSelectItem()
    {
        if (m_selectSlot == null)
            return null;

        return m_selectSlot.m_currentItem;
    }

    public override void OnMouseDownEvent()
    {
        if (m_grid == null || m_heldItem != null)
            return;

        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                if (m_grid[c, r].IsContainingVector(InputManager.instance.GetMousePositionInScreen()))
                {
                    m_heldItem = m_grid[c, r];
                    m_heldItem.OnMouseDownEvent();
                }
            }
        }
    }

    public override void OnMouseUpEvent()
    {
        if (m_grid == null || m_heldItem == null)
            return;

        //Check own grid:
        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                if (m_grid[c, r].IsContainingVector(InputManager.instance.GetMousePositionInScreen()))
                {
                    m_heldItem.TransferItemTo(m_grid[c, r]);
                    m_heldItem.OnMouseUpEvent();
                    m_heldItem = null;
                    m_hasUpdated = true;
                    FindObjectOfType<PlayerController>().GetComponent<PlayerPlacing>().SetSelectedIndex(-1);
                    return;
                }
            }
        }
        //Must be another grid:
        UI_Element otherSystem = HUDManager.instance.GetElementUnderMouse();
        if(otherSystem != null)
        {
            (otherSystem as UI_GridDisplay).SwitchItem(m_heldItem);
            FindObjectOfType<PlayerController>().GetComponent<PlayerPlacing>().SetSelectedIndex(-1);
            m_heldItem.OnMouseUpEvent();
            m_heldItem = null;
            m_hasUpdated = true;
            return;
        }

        //Drop
        if(m_heldItem.m_currentItem.GetToolType() == ToolType.Null)
        {
            Transform trans = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().m_playerModel.transform;
            LootDrop.CreateLoot(m_heldItem.m_currentItem.m_id, m_heldItem.m_currentItem.m_amount, trans.position + trans.forward * 2.0f + trans.up);
            FindObjectOfType<PlayerController>().GetComponent<PlayerPlacing>().SetSelectedIndex(-1);
            m_heldItem.SetItem(null);
            m_heldItem.OnMouseUpEvent();
            m_heldItem = null;
            m_hasUpdated = true;
            return;
        }
        
        m_heldItem.OnMouseUpEvent();
        m_heldItem = null;
    }

    public bool SwitchItem(UI_SlotDisplay fromSlot)
    {
        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                if (m_grid[c, r].IsContainingVector(InputManager.instance.GetMousePositionInScreen()))
                {
                    fromSlot.TransferItemTo(m_grid[c, r]);
                    m_hasUpdated = true;
                    return true;
                }
            }
        }
        return false;
    }
    public void SelectItem(int column, int row)
    {
        if(column <= -1 || row <= -1)
        {
            m_selectSlot?.Unselect();
            m_selectSlot = null;
            return;
        }

        m_selectSlot?.Unselect();
        m_selectSlot = m_grid[column, row];
        m_selectSlot.Select();
    }
    public void Clear()
    {
        if (m_grid != null)
            for (int c = 0; c < m_columnCount; c++)
            {
                for (int r = 0; r < m_rowCount; r++)
                {
                    Destroy(m_grid[c, r].gameObject);
                }
            }
        m_grid = null;
    }
}
