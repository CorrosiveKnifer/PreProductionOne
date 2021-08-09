using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * UI_SlotDisplay by Michael Jordan
 * File: UI_SlotDisplay.cs
 * Description:
 *		Used to manage the individual slot within a grid of slots.
 */

public class UI_SlotDisplay : UI_Element
{
    public ItemObject m_currentItem { get; protected set; }
    
    [Header("Dependencies")]
    public bool m_isShowingDependencies = true;
    [ShowIf("m_isShowingDependencies")]
    public Image m_buttonImage;
    [ShowIf("m_isShowingDependencies")]
    public Image m_itemImage;
    [ShowIf("m_isShowingDependencies")]
    public Text m_amountText;
    [ShowIf("m_isShowingDependencies")]
    public Image m_amountBar;
    [ShowIf("m_isShowingDependencies")]
    public Canvas m_itemCanvas;

    private bool m_showbar = false;
    // Start is called before the first frame update
    private void Start()
    {
        m_isShowingDependencies = false;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateDimentions();

        m_showbar = m_currentItem != null && m_currentItem.GetToolType() == ToolType.WaterCan;

        m_itemImage.gameObject.SetActive(m_currentItem != null);
        m_amountText.gameObject.SetActive(!m_showbar);
        m_amountBar.gameObject.SetActive(m_showbar);

        if (m_currentItem != null && m_currentItem.m_type == ItemType.Tool)
        {
            m_amountText.text = "";
            m_amountBar.fillAmount = 0;
        }
        else if(m_amountText.isActiveAndEnabled)
        {
            m_amountText.text = m_currentItem.m_amount.ToString();
        }
        if(m_amountBar.isActiveAndEnabled)
        {
            m_amountBar.fillAmount = Mathf.Clamp((m_currentItem.m_amount - 1) / (ItemObject.MAX_AMOUNT - 1), 0, 1.0f);
        }
    }

    /*
     * UpdateDimentions by Michael Jordan
     * Description:
     *  Used to update the slot's scale relative to the location provided.
     *
     * Param:
     *
     */
    public void UpdateDimentions()
    {
        RectTransform myTransform = GetComponent<RectTransform>();
        RectTransform textTransform = m_amountText.gameObject.GetComponent<RectTransform>();
        RectTransform imageTransform = m_itemImage.gameObject.GetComponent<RectTransform>();

        textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (myTransform.rect.width * 0.8f) / 2.0f);
        textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (myTransform.rect.height * 0.8f ) / 2.0f);

        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, myTransform.rect.width * 0.8f);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, myTransform.rect.height * 0.8f);
    }

    public void MoveImageTo(Vector2 _pos)
    {
        m_itemImage.GetComponent<RectTransform>().position = _pos;
    }

    public bool IsVector2Within()
    {
        return false;
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        RectTransform myTransform = GetComponent<RectTransform>();

        return _pos.x > (myTransform.position.x + myTransform.rect.xMin)
            && _pos.x < (myTransform.position.x + myTransform.rect.xMax)
            && _pos.y > (myTransform.position.y + myTransform.rect.yMin)
            && _pos.y < (myTransform.position.y + myTransform.rect.yMax);
    }

    public void SetItem(ItemObject _itemObject)
    {
        m_currentItem = _itemObject;
        if(m_currentItem != null)
        {
            m_itemImage.sprite = _itemObject.GetSprite();
            m_amountText.text = _itemObject.m_amount.ToString();
        }
        else
        {
            m_itemImage.sprite = null;
            m_amountText.text = "0";
        }
    }

    public override void OnMouseDownEvent()
    {
        m_itemCanvas.sortingOrder = 10;
    }

    public override void OnMouseUpEvent()
    {
        RectTransform imageTransform = m_itemImage.gameObject.GetComponent<RectTransform>();
        m_itemCanvas.sortingOrder = 1;
        imageTransform.anchoredPosition = Vector2.zero;
    }

    public void TransferItemTo(UI_SlotDisplay uI_SlotDisplay)
    {
        if (uI_SlotDisplay == this)
            return;

        ItemObject temp = uI_SlotDisplay.m_currentItem;
        
        if(uI_SlotDisplay.m_currentItem != null && m_currentItem != null)
        {
            if (uI_SlotDisplay.m_currentItem.m_id == m_currentItem.m_id && uI_SlotDisplay.m_currentItem.m_amount + m_currentItem.m_amount <= ItemObject.MAX_AMOUNT)
            {
                uI_SlotDisplay.m_currentItem.m_amount += m_currentItem.m_amount;
                this.SetItem(null);
                return;
            }
            else if(uI_SlotDisplay.m_currentItem.m_id == m_currentItem.m_id)
            {
                uint diff = (uI_SlotDisplay.m_currentItem.m_amount + m_currentItem.m_amount) - (uint)ItemObject.MAX_AMOUNT;
                uI_SlotDisplay.m_currentItem.m_amount = (uint)ItemObject.MAX_AMOUNT;
                this.SetItem(ItemObject.CreateItem(uI_SlotDisplay.m_currentItem.m_id, diff));
                return;
            }
        }
        
        uI_SlotDisplay.SetItem(m_currentItem);
        this.SetItem(temp);
    }

    public ItemObject GetItem()
    {
        return m_currentItem;
    }
    public void Select()
    {
        m_buttonImage.color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
    }

    public void Unselect()
    {
        m_buttonImage.color = Color.white;
    }
}
