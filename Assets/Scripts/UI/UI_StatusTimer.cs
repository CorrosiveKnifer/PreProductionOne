using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusTimer : UI_Element
{
    [SerializeField] private Image m_display;
    [SerializeField] private Image m_icon;
    private float m_fill;
    public statusType m_status;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetStatus(statusType _status)
    {
        m_status = _status;

        switch (m_status)
        {
            case statusType.OVER_FILLED:
                m_icon.sprite = Resources.Load<Sprite>("Sprites/TEMP/Anvil");
                break;
            case statusType.TEST:
                m_icon.sprite = Resources.Load<Sprite>("Sprites/TEMP/Test");
                break;
            default:
                break;
        }
    }

    public void SetFill(float _fill)
    {
        m_display.fillAmount = _fill;
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
