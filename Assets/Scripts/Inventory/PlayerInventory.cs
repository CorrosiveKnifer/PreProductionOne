using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PlayerInventory
{
    [Range(1, 20)]
    public int m_rowCount = 5;
    [Range(1, 20)]
    public int m_columnCount = 5;

    private Item[,] m_itemGrid;


    public PlayerInventory()
    {
        m_itemGrid = new Item[m_columnCount, m_rowCount];
    }
}
