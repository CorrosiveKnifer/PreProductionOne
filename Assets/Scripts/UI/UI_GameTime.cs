using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameTime : UI_Element
{
    public SunScript m_sunTimer;

    [SerializeField] private Text m_display;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float time = GameManager.instance.m_currentHour;
        int minutes = (int)Mathf.Floor((time - Mathf.Floor(time)) * 60.0f);
        string minuteDisplay = (minutes >= 10) ? minutes.ToString() : "0"+ minutes.ToString();
        if (time > 0 && time < 12)
        {
            m_display.text = $"Day: {GameManager.instance.m_day} - {(int)Mathf.Floor(time)}:{minuteDisplay} AM";
        }
        else if(time >= 12 && time <= 24)
        {
            m_display.text = $"Day: {GameManager.instance.m_day} - {(int)Mathf.Floor(time) - 12}:{minuteDisplay} PM";
        }
        else
        {
            m_display.text = $"Day: {GameManager.instance.m_day} - 12:{minuteDisplay} AM";
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
