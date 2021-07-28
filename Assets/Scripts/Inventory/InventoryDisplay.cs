using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    [Range(1, 20)]
    public int m_rowCount = 5;
    [Range(1, 20)]
    public int m_columnCount = 5;
    [Range(0.0f, 1.0f)]
    public float m_itemScale = 1.0f;

    public Sprite m_buttonBaseSprite;
    public Sprite m_tempItemSprite;

    private GameObject[,] m_grid; //[x, y]
    private Image heldImage = null;

    // Start is called before the first frame update
    void Start()
    {
        m_grid = new GameObject[m_columnCount, m_rowCount];

        for (int c = 0; c < m_columnCount; c++)
        {
            for (int r = 0; r < m_rowCount; r++)
            {
                SetUpCell(c, r);
            }
        }

        GridLayoutGroup layout = gameObject.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(GetComponent<RectTransform>().rect.width / m_columnCount, GetComponent<RectTransform>().rect.height / m_rowCount);
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.GetMouseButtonPressed(MouseButton.RIGHT))
        {
            Vector2 mousePos = InputManager.instance.GetMousePosition();
            if (heldImage == null)
            {
                foreach (var item in GetComponentsInChildren<Button>())
                {
                    if (item.gameObject.GetComponent<RectTransform>().rect.Contains(mousePos))
                    {
                        heldImage = item.GetComponentsInChildren<Image>()[1];
                    }
                }
            }
            else
            {
                heldImage.transform.position = mousePos;
            }
            
        }
        else
        {
            heldImage = null;
        }

    }

    public bool SetSpriteInCell(int column, int row, Sprite newSprite)
    {
        if(column >= m_columnCount || row > m_rowCount)
        {
            throw new IndexOutOfRangeException($"Either column or row was larger than the specified range in InventoryDisplay.");
        }

        foreach (var item in m_grid[column, row].GetComponentsInChildren<Image>())
        {
            if(item.gameObject.name.Contains("Image"))
            { 
                item.sprite = newSprite;
                return true;
            }
        }
        return false;
    }

    private void SetUpCell(int c, int r)
    {
        RectTransform parentRect = GetComponent<RectTransform>();

        //Inital position of 0,0
        float cellStartX = -parentRect.rect.width / 2;
        float cellStartY = parentRect.rect.height / 2;

        //Size of each cell
        float cellWidth = parentRect.rect.width / m_columnCount;
        float cellHeight = parentRect.rect.height / m_rowCount;

        //This cell's position relative to 0,0
        float cellX = c * cellWidth + cellWidth/2.0f + cellStartX;
        float cellY = -(r + 1) * cellHeight + cellHeight / 2.0f + cellStartY;

        //Create the object:
        m_grid[c, r] = new GameObject();

        //Parent it for easy collapse:
        m_grid[c, r].transform.parent = transform;
        m_grid[c, r].name = $"Cell[{c},{r}]";

        //Add a RectTransform to the game object and update it's location.
        RectTransform cellRect = m_grid[c, r].AddComponent<RectTransform>();
        cellRect.anchoredPosition = new Vector2(cellX, cellY);
        cellRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellWidth);
        cellRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellHeight);
        
        Image cellImage = m_grid[c, r].AddComponent<Image>();
        cellImage.sprite = m_buttonBaseSprite;

        Button cellButton =  m_grid[c, r].AddComponent<Button>();
        cellButton.targetGraphic = cellImage;
        
        GameObject subObject = new GameObject();
        subObject.name = $"Image[{c},{r}]";
        subObject.transform.parent = m_grid[c, r].transform;

        RectTransform subRect = subObject.AddComponent<RectTransform>();
        subRect.anchoredPosition = Vector2.zero;

        subRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellWidth * m_itemScale);
        subRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellHeight * m_itemScale);

        Image subImage = subObject.AddComponent<Image>();
        subImage.sprite = m_tempItemSprite;
    }
}
