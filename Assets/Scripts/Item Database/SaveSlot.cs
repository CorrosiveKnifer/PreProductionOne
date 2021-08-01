using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveSlot
{
    [Serializable]
    public class Save_Item
    {
        public int m_id = -1;
        public int m_quantity = 0;
    }

    [Serializable]
    public class Save_Player
    {
        public int m_rows;
        public int m_columns;
        public int m_hotbarCount;

        public Save_Item[] m_backpack;
        public Save_Item[] m_hotbar;

        public Save_Player()
        {
            m_rows = 5;
            m_columns = 5;
            m_hotbarCount = 5;
            m_backpack = new Save_Item[m_columns * m_rows];
            m_hotbar = new Save_Item[m_hotbarCount];
        }
    }

    [Serializable]
    public class Save
    {
        public Save_Player m_player;

        public Save()
        {
            m_player = new Save_Player();
        }
    }

    protected Save savedData;
    
    public SaveSlot()
    {
        savedData = new Save();
    }

    public SaveSlot(string jsonText)
    {
        savedData = JsonUtility.FromJson<Save>(jsonText);
    }

    public void SaveToFile(string file)
    {
        File.WriteAllText(file, JsonUtility.ToJson(savedData));
    }

    public ItemObject GetPlayerBackpackData(int _column, int _row)
    {
        int width = savedData.m_player.m_columns;
        Save_Item item = savedData.m_player.m_backpack[_column * width + _row];

        if (item.m_quantity < 0)
            return null;

        return ItemObject.CreateItem(item.m_id, (uint)item.m_quantity);
    }

    public ItemObject GetPlayerHotbarData(int _column)
    {
        Save_Item item = savedData.m_player.m_hotbar[_column];

        if (item.m_quantity < 0)
            return null;

        return ItemObject.CreateItem(item.m_id, (uint)item.m_quantity);
    }

    public int GetPlayerIntegerData(string dataName)
    {
        switch (dataName.ToLower())
        {
            case "backpack_column":
                return savedData.m_player.m_columns;
            case "backpack_row":
                return savedData.m_player.m_rows;
            case "hotbar_column":
                return savedData.m_player.m_hotbarCount;
            default:
                return 0;
        }
    }
    public void SavePlayerBackpackData(int _column, int _row, int _id, uint _quantity)
    {
        int width = savedData.m_player.m_columns;
        Save_Item item = savedData.m_player.m_backpack[_column * width + _row];
        item.m_id = _id;
        item.m_quantity = (int)_quantity;
    }
    public void SavePlayerHotbarData(int _column, int _id, uint _quantity)
    { 
        Save_Item item = savedData.m_player.m_hotbar[_column];
        item.m_id = _id;
        item.m_quantity = (int)_quantity;
    }
}
