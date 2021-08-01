using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HungerBar : UI_Element
{
    public PlayerVitality m_playerVitality;
    private float m_hunger;

    [SerializeField] private Image m_display;
    [SerializeField] private GameObject m_timerPrefab;
    private List<UI_StatusTimer> m_timers = new List<UI_StatusTimer>();
    private int m_timerSpacing = 65;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_hunger = m_playerVitality.GetHunger();
        m_display.fillAmount = m_hunger / 100.0f;
    }

    public void CreateTimer(statusType _status)
    {
        foreach (var timer in m_timers)
        {
            if (timer.m_status == _status)
            {
                return;
            }
        }

        UI_StatusTimer newTimer = Instantiate(m_timerPrefab, transform).GetComponent<UI_StatusTimer>();
        m_timers.Add(newTimer);
        newTimer.SetStatus(_status);
        newTimer.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(-40.0f, 80.0f + m_timerSpacing * m_timers.Count);
    }

    public void UpdateTimers(List<statusEffect> _statusList)
    {
        foreach (var timer in m_timers)
        {
            foreach (var status in _statusList)
            {
                if (timer.m_status == status.effect)
                {
                    timer.SetFill(status.duration / status.startDuration);
                }
            }
        }
    }

    public void DestroyTimer(statusType _status)
    {
        bool removed = false;
        List<UI_StatusTimer> removeList = new List<UI_StatusTimer>();
        foreach (var timer in m_timers)
        {
            if (timer.m_status == _status)
            {
                removeList.Add(timer); // Add effects to be removed
                removed = true;
            }
            if (removed)
            {
                timer.transform.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, m_timerSpacing);
            }
        }

        foreach (var timer in removeList)
        {
            GameObject obj = timer.gameObject;
            m_timers.Remove(timer);
            Destroy(obj);
        }
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        //Do Nothing
    }

    public override void OnMouseUpEvent()
    {
        //Do Nothing
    }
}
