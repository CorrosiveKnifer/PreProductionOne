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
    public Vector2Int m_spacing;

    [Range(1, 20)]
    public int m_rowCount = 5;
    [Range(1, 20)]
    public int m_columnCount = 5;
    [Range(0.0f, 1.0f)]
    public float m_itemScale = 1.0f;

    public GameObject m_buttonPrefab;

    private UI_SlotDisplay[,] m_grid; //[x, y]
    private Image heldImage = null;

    // Start is called before the first frame update
    void Start()
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
        Debug.Log(m_grid[1, 0].GetComponent<RectTransform>().position.x);
        //if(InputManager.instance.GetMouseButtonPressed(MouseButton.RIGHT))
        //{
        //    Vector2 mousePos = InputManager.instance.GetMousePosition();
        //    if (heldImage == null)
        //    {
        //        foreach (var item in GetComponentsInChildren<Button>())
        //        {
        //            if (item.gameObject.GetComponent<RectTransform>().rect.Contains(mousePos))
        //            {
        //                heldImage = item.GetComponentsInChildren<Image>()[1];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        heldImage.transform.position = mousePos;
        //    }
        //    
        //}
        //else
        //{
        //    heldImage = null;
        //}
    }

    public bool SetSpriteInCell(int column, int row, Sprite newSprite)
    {
        //if(column >= m_columnCount || row > m_rowCount)
        //{
        //    throw new IndexOutOfRangeException($"Either column or row was larger than the specified range in InventoryDisplay.");
        //}
        //
        //foreach (var item in m_grid[column, row].GetComponentsInChildren<Image>())
        //{
        //    if(item.gameObject.name.Contains("Image"))
        //    { 
        //        item.sprite = newSprite;
        //        return true;
        //    }
        //}
        return false;
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                if (m_grid[c, r].IsContainingVector(_pos))
                {
                    Debug.Log($"{m_grid[c, r].name}");
                    return true;
                }
            }
        }
        return false;
    }
}
