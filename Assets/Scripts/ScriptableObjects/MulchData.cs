using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mulch Data", menuName = "ScriptableObjects/Mulch Data", order = 2)]
public class MulchData : ScriptableObject
{
    [Range(0.0f, 1.0f)]
    public float m_water = 0;
    public float m_age = 0;
}
