using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance = null;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject loader = new GameObject();
                _instance = loader.AddComponent<GameManager>();
                return loader.GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_instance == this)
        {
            InitialiseFunc();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Second Instance of GameManager was created, this instance was destroyed.");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    #endregion

    public ItemDB m_items;
    public DoorDirection m_TargetDoor;
    private TextAsset m_itemsJson;
    public SaveSlot m_saveSlot;
    public float m_currentHour = 8;
    public int m_day = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.IsKeyDown(KeyType.T))
        {
            SkipTime(8.0f);
        }
    }

    private void InitialiseFunc()
    {
        gameObject.name = "Game Manager";
        m_itemsJson = Resources.Load<TextAsset>("Items");
        m_items = JsonUtility.FromJson<ItemDB>(m_itemsJson.text);

        if(File.Exists(Application.dataPath + "/SaveSlot1.json"))
        {
            m_saveSlot = new SaveSlot(File.ReadAllText(Application.dataPath + "/SaveSlot1.json"));
        }
        else
        {
            File.Create(Application.dataPath + "/SaveSlot1.json");
            Debug.Log($"SaveSlot doesn't exist, it was created in {Application.dataPath}/");
            m_saveSlot = new SaveSlot();
        }
        
    }

    public void SkipTime(float hoursIncreased)
    {
        m_currentHour += hoursIncreased;
        if (m_currentHour >= 24)
        {
            m_currentHour -= 24;
            m_day++;
        }
    }
    public void OnApplicationQuit()
    {
        m_saveSlot.SaveToFile(Application.dataPath + "/SaveSlot1.json");
    }
}
