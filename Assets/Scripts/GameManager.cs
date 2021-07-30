using System.Collections;
using System.Collections.Generic;
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
    public TextAsset m_itemsJson;

    public float m_currentHour = 8;
    public int m_day = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitialiseFunc()
    {
        m_items = new ItemDB(m_itemsJson.text);
    }

    public void SkipTime(float hoursIncreased)
    {
        m_currentHour += hoursIncreased;
        if (m_currentHour > 23)
        {
            m_currentHour -= 23;
            m_day++;
        }
            
    }
}
