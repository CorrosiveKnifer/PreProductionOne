using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogSection
{
    [Serializable]
    public class DialogText
    {
        public string text;
        public string image;
    }

    public DialogText[] greeting;
    public DialogText noQuest;
    public DialogText failedQuest;
}
public class QuestNPC : NPCScript
{
    public TextAsset m_dataFile;

    public GameObject m_questMarkerPrefab;
    public int m_hoursPerQuest;
    public float m_nextQuest;

    [Header("Dialog")]
    public Sprite m_myImage = null;
    public string m_myName = "";
    public string m_questText = "";
    public string m_noQuestText = "";
    public float m_heightOffset = 0.5f;

    private DialogSection m_data;
    private GameObject m_questMarker;
    private Quest m_myQuest;

    private List<Dialog> m_greetingDialog;
    private Dialog m_questDialog;
    private Dialog m_noQuestDialog;
    private UI_DialogSystem m_system;

    private float m_currentDay;
    private float m_currentHour;

    public void Awake()
    {
        m_data = JsonUtility.FromJson<DialogSection>(m_dataFile.text);
    }

    protected override void Start()
    {
        m_questMarker = Instantiate(m_questMarkerPrefab, transform);
        m_questMarker.transform.position = m_questMarker.transform.position + new Vector3(0, m_heightOffset, 0);

        m_questDialog = new Dialog(m_myImage, m_myName, m_questText);
        m_system = HUDManager.instance.GetElementByType(typeof(UI_DialogSystem)) as UI_DialogSystem;

        InitialiseGreetingDialog();
        GenerateRandomQuest();
    }

    protected override void Update()
    {
        m_questMarker.SetActive(m_myQuest != null);

        if(m_nextQuest > 0)
        {
            m_nextQuest -= GetInGameDeltaHours();
        }
        else if(m_nextQuest <= 0)
        {
            if (m_myQuest == null)
                GenerateRandomQuest();
        }
        m_currentDay = GameManager.instance.m_day;
        m_currentHour = GameManager.instance.m_currentHour;
    }

    private float GetInGameDeltaHours()
    {
        float oldHour = m_currentHour;
        float nextHour = GameManager.instance.m_currentHour;
        if(m_currentDay < GameManager.instance.m_day)
        {
            nextHour += 23.0f;
        }

        return nextHour - oldHour;
    }

    public void NextDialog()
    {
        if (m_myQuest != null)
        {
            m_system.LoadDialog(m_questDialog);
        }
        else
        {
            m_system.LoadDialog(m_noQuestDialog);
        }
    }

    public void RedeemQuest()
    {
        int redeemed = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuests>().RedeemQuests();
        Dialog reply;
        if (redeemed == -1)
        {
            reply = new Dialog(m_myImage, m_myName, "Odd, I haven't given you a quest yet.");
        }
        else if (redeemed > 0)
        {
            reply = new Dialog(m_myImage, m_myName, $"Thank you! ({redeemed} quests has been redeemed).");
        }
        else
        {
            reply = new Dialog(m_myImage, m_myName, "Unfortunate.");
        }
        m_system.LoadDialog(reply);
    }

    public void AcceptQuest()
    {
        Dialog reply = new Dialog(m_myImage, m_myName, "Thank you, I can't wait.");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuests>().AddQuest(m_myQuest);
        m_myQuest = null;
        m_nextQuest = m_hoursPerQuest;
        m_system.LoadDialog(reply);
    }

    public void DeclineQuest()
    {
        Dialog reply = new Dialog(m_myImage, m_myName, "Unfortunate.");
        m_system.LoadDialog(reply);
        m_myQuest = null;
        m_nextQuest = m_hoursPerQuest;
    }

    public void StopDialog()
    {
        m_system.gameObject.SetActive(false);
        
    }

    public void InteractWith()
    {
        //m_system.gameObject.SetActive(true);
        //m_system.LoadDialog(m_greetingDialog[Random.Range(0, m_greetingDialog.Count - 1)]);
    }

    public void GenerateRandomQuest()
    {
        //m_myQuest = new Quest(Random.Range(0, 4), Random.Range(5, 15), Random.Range(1, 7));
        //string itemName = GameManager.instance.m_items.list[m_myQuest.m_itemId].name;
        //m_questDialog.dialog = $"Can I get {m_myQuest.m_amount} {itemName.ToLower()} within {m_myQuest.GetRemainingDays()} days?";
    }

    private void InitialiseGreetingDialog()
    {
        //Dictionary<string, System.Action> m_responceOptions = new Dictionary<string, System.Action>();
        //m_responceOptions.Add("Get a Quest", NextDialog);
        //m_responceOptions.Add("Redeem a Quest", RedeemQuest);
        //
        //m_greetingDialog = new List<Dialog>();
        //
        //foreach (var greeting in m_randomGreetings)
        //{
        //    m_greetingDialog.Add(new Dialog(m_myImage, m_myName, greeting, m_responceOptions));
        //}
        //
        //m_noQuestDialog = new Dialog(m_myImage, m_myName, m_noQuestText);
        //
        //Dictionary<string, System.Action> m_questOptions = new Dictionary<string, System.Action>();
        //m_questOptions.Add("Sure I can!", AcceptQuest);
        //m_questOptions.Add("Sorry I can't", DeclineQuest);
        //
        //m_questDialog = new Dialog(m_myImage, m_myName, "blank quest", m_questOptions);
    }

    public override string GetExtraData()
    {
        return $"{m_nextQuest}";
    }

    public override void SetExtraData(string data)
    {
        m_nextQuest = int.Parse(data);
    }
}
