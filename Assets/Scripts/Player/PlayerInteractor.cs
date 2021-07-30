﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private Camera m_PlayerCamera;
    Interactable m_selectedInteractable;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSelected(Interactable _interactable, bool _selected)
    {
        if (m_selectedInteractable == _interactable && !_selected)
        {
            Debug.Log("Deselected interactable");
            m_selectedInteractable = null;
            // Remove any associated HUD
        }
        else if (m_selectedInteractable == null && _selected)
        {
            Debug.Log("Selected interactable");
            m_selectedInteractable = _interactable;
            // Enable any associated HUD
        }
    }

    public void InteractWithObject()
    {
        if (m_selectedInteractable != null)
            m_selectedInteractable.Interact();
    }
}
