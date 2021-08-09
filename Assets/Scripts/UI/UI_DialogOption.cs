using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogOption : UI_Element
{
    public string m_optionText = "Option";

    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<Text>().text = m_optionText;
    }
    
    public void OnClick()
    {
        GetComponentInParent<UI_DialogSystem>().OnOptionClickEvent(m_optionText);
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
