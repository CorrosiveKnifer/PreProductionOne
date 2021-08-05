using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            m_rows = 3;
            m_columns = 5;
            m_hotbarCount = 5;
            m_backpack = new Save_Item[m_columns * m_rows];

            for (int i = 0; i < m_columns; i++)
            {
                for (int j = 0; j < m_rows; j++)
                {
                    m_backpack[i * m_columns + j] = new Save_Item();
                }
            }

            m_hotbar = new Save_Item[m_hotbarCount];
            for (int i = 0; i < m_hotbarCount; i++)
            {
                m_hotbar[i] = new Save_Item();
            }
        }
    }
    [Serializable]
    public class Save_Scene
    {
        public int m_id;
        public int m_objectCount;
        public SerializedData[] m_objects;

    }

    [Serializable]
    public class Save
    {
        public Save_Player m_player;
        public Save_Scene[] m_scenes;

        public int m_day = 0;
        public float m_hour = 6.0f;

        public Save()
        {
            m_player = new Save_Player();

            m_scenes = new Save_Scene[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < m_scenes.Length; i++)
            {
                m_scenes[i] = new Save_Scene();
                m_scenes[i].m_id = i;
                m_scenes[i].m_objectCount = 0;
            }
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

    public void SaveObjects(GameObject[] gameObjects)
    {
        if(gameObjects == null || gameObjects.Length == 0)
        {
            return;
        }

        //If it doesn't exist, create the data
        if(savedData.m_scenes == null || savedData.m_scenes.Length != SceneManager.sceneCountInBuildSettings)
        {
            savedData.m_scenes = new Save_Scene[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < savedData.m_scenes.Length; i++)
            {
                savedData.m_scenes[i] = new Save_Scene();
                savedData.m_scenes[i].m_id = i;
                savedData.m_scenes[i].m_objectCount = 0;
            }
        }

        //Get current build index
        int id = SceneManager.GetActiveScene().buildIndex;

        //If this is the first save:
        if (savedData.m_scenes[id].m_objectCount != gameObjects.Length)
        {
            savedData.m_scenes[id].m_objects = new SerializedData[gameObjects.Length];
        }

        //For each object:
        for (int i = 0; i < gameObjects.Length; i++)
        {
            //Check if it has the correct component:
            if(gameObjects[i].GetComponentInChildren<SerializedObject>() != null)
            {
                //Set Data:
                savedData.m_scenes[id].m_objects[i] = gameObjects[i].GetComponentInChildren<SerializedObject>().data;
            }
        }

        //Update length:
        savedData.m_scenes[id].m_objectCount = savedData.m_scenes[id].m_objects.Length;
    }

    public SerializedData[] GetSceneData(int buildIndex)
    {
        if (savedData.m_scenes == null || savedData.m_scenes.Length != SceneManager.sceneCountInBuildSettings)
        {
            savedData.m_scenes = new Save_Scene[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < savedData.m_scenes.Length; i++)
            {
                savedData.m_scenes[i] = new Save_Scene();
                savedData.m_scenes[i].m_id = i;
                savedData.m_scenes[i].m_objectCount = 0;
            }
        }

        return savedData.m_scenes[buildIndex].m_objects;
    }

    public float GetSaveHour()
    {
        return savedData.m_hour;
    }
    public int GetSaveDay()
    {
        return savedData.m_day;
    }
    public void SetTime(int day, float hour)
    {
        savedData.m_day = day;
        savedData.m_hour = hour;
    }
}
