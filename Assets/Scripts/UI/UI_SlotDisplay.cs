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
    [SerializeField] private ItemObject m_currentItem = null;
    
    [Header("Dependencies")]
    public bool m_isShowingDependencies = true;
    [ShowIf("m_isShowingDependencies")]
    public Image m_buttonImage;
    [ShowIf("m_isShowingDependencies")]
    public Image m_itemImage;
    [ShowIf("m_isShowingDependencies")]
    public Text m_amountText;
    [ShowIf("m_isShowingDependencies")]
    public Canvas m_itemCanvas;

    // Start is called before the first frame update
    private void Start()
    {
        m_isShowingDependencies = false;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateDimentions();
        m_itemImage.gameObject.SetActive(m_currentItem != null);
        m_amountText.gameObject.SetActive(m_currentItem != null);
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
            m_itemImage.sprite = _itemObject.m_definition.inventoryImage;
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
        ItemObject temp = uI_SlotDisplay.m_currentItem;
        uI_SlotDisplay.SetItem(m_currentItem);
        this.SetItem(temp);
    }

    public ItemObject GetItem()
    {
        return m_currentItem;
    }
}
