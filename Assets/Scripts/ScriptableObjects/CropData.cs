using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CropDrop
{
    public int m_itemDropID;

    public bool m_isRandom;

    public int m_minDropAmount;
    public int m_maxDropAmount;
}

[CreateAssetMenu(fileName ="Crop Data", menuName = "ScriptableObjects/Crop Data", order = 1)]
public class CropData : ScriptableObject
{
    [Header("Plant stats")]
    
    [Tooltip("Maximum age of this plant.")]
    public int m_maxAge = 7;

    [Tooltip("Time required while at max age till a harvest.")]
    public int m_timeRequired = 0;

    [Tooltip("If the plant is not destroyed upon harvesting.")]
    public Vector3 m_maxHeight;

    [Range(0.0f, 2.0f)]
    [Tooltip("Min variance to the max height.")]
    public float m_minVariance = 0.85f;

    [Range(0.0f, 2.0f)]
    [Tooltip("Max variance to the max height.")]
    public float m_maxVariance = 1.15f;

    [Tooltip("If the plant is not destroyed upon harvesting.")]
    public bool m_isRenewable = false;

    [Header("Water stats")]

    [Range(0.0f, 1.0f)]
    [Tooltip("Water value required for this plant to age.")]
    public float m_waterRequirement = 0.25f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Water decayed upon each day of growth.")]
    public float m_waterDecay = 0.5f;

    [Header("Loot")]
    [Tooltip("Loot dropped upon harvesting this plant.")]
    public CropDrop[] m_drops;
}
