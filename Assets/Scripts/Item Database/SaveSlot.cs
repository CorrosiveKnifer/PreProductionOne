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
    public class Save_NPC
    {
        public int m_id;
        public int m_scene;
        public string m_prefab;
        public float x;
        public float y;
        public float z;
        public float rx;
        public float ry;
        public float rz;
        public string m_extraData;

        public Save_NPC(NPCScript npc)
        {
            m_id = npc.id;
            m_scene = SceneManager.GetActiveScene().buildIndex;
            m_prefab = npc.gameObject.name;
            x = npc.transform.position.x;
            y = npc.transform.position.y;
            z = npc.transform.position.z;
            rx = npc.transform.rotation.eulerAngles.x;
            ry = npc.transform.rotation.eulerAngles.y;
            rz = npc.transform.rotation.eulerAngles.z;
            m_extraData = npc.GetExtraData();
        }
    }

    [Serializable]
    public class Save_Player
    {
        public int m_rows;
        public int m_columns;
        public int m_hotbarCount;

        public int m_questsCompleted;
        public int m_questsFailed;

        public Save_Item[] m_backpack;
        public Save_Item[] m_hotbar;

        public Save_Player()
        {
            m_rows = 3;
            m_columns = 5;
            m_hotbarCount = 5;
            m_questsCompleted = 0;
            m_questsFailed = 0;
            m_backpack = new Save_Item[m_columns * m_rows];

            for (int i = 0; i < m_columns; i++)
            {
                for (int j = 0; j < m_rows; j++)
                {
                    m_backpack[i * m_rows + j] = new Save_Item();
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
    public class Save_Quest
    {
        public int m_dueDay;
        public int m_itemId;
        public int m_itemAmount;

        public Save_Quest(Quest _data)
        {
            m_dueDay = _data.m_dueDay;
            m_itemId = _data.m_itemId;
            m_itemAmount = _data.m_amount;
        }
        public bool IsEqual(Quest _data)
        {
            return m_dueDay == _data.m_dueDay && m_itemId == _data.m_itemId && m_itemAmount == _data.m_amount;
        }
    }

    public void SetQuests(int m_questsDone, int m_questsFailed)
    {
        savedData.m_player.m_questsCompleted = m_questsDone;
        savedData.m_player.m_questsFailed = m_questsFailed;
    }
    public int GetQuestsData(int dataIndex)
    {
        switch (dataIndex)
        {
            default: return 0;
            case 0: return savedData.m_player.m_questsCompleted;
            case 1: return savedData.m_player.m_questsFailed;
        }
    }

    [Serializable]
    public class Save
    {
        public Save_Player m_player;
        public Save_Scene[] m_scenes;
        public List<Save_Quest> m_quests;
        public List<Save_NPC> m_npcs;

        public int m_day = 0;
        public float m_hour = 6.0f;

        public Save()
        {
            m_player = new Save_Player();

            m_scenes = new Save_Scene[SceneManager.sceneCountInBuildSettings];
            m_quests = new List<Save_Quest>();
            m_npcs = new List<Save_NPC>();

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
        if (jsonText == string.Empty)
        {
            savedData = new Save();
            return;
        }

        savedData = JsonUtility.FromJson<Save>(jsonText);
    }

    public void SaveToFile(string file)
    {
        File.WriteAllText(file, JsonUtility.ToJson(savedData));
    }

    public ItemObject GetPlayerBackpackData(int _column, int _row)
    {
        int width = savedData.m_player.m_rows;
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
        int width = savedData.m_player.m_rows;
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

    public void AddQuest(Quest quest)
    {
        savedData.m_quests.Add(new Save_Quest(quest));
    }

    public void RemoveQuest(Quest quest)
    {
        for (int i = 0; i < savedData.m_quests.Count; i++)
        {
            if(savedData.m_quests[i].IsEqual(quest))
            {
                savedData.m_quests.RemoveAt(i);
                break;
            }
        }
    }

    public List<Quest> GetQuests()
    {
        List<Quest> list = new List<Quest>();
        foreach (var item in savedData.m_quests)
        {
            list.Add(new Quest(item.m_itemId, item.m_itemAmount, item.m_dueDay));
        }
        return list;
    }

    public void InstansiateNPCs(int sceneIndex)
    {
        List<Save_NPC> toRemove = new List<Save_NPC>();
        foreach (var npc in savedData.m_npcs)
        {
            if(npc.m_scene == sceneIndex)
            {
                Vector3 pos = new Vector3(npc.x, npc.y, npc.z);
                Quaternion rotation = Quaternion.Euler(npc.rx, npc.ry, npc.rz);
                GameObject prefab = Resources.Load<GameObject>($"prefab/{npc.m_prefab}");

                if (prefab != null)
                {
                    NPCScript script = GameObject.Instantiate(prefab, pos, rotation).GetComponentInChildren<NPCScript>();
                    script.SetExtraData(npc.m_extraData);
                }
                toRemove.Add(npc);
            }
        }
        foreach (var item in toRemove)
        {
            savedData.m_npcs.Remove(item);
        }
    }

    public void AddNPC(NPCScript script)
    {
        if(script != null)
            savedData.m_npcs.Add(new Save_NPC(script));
    }
}
