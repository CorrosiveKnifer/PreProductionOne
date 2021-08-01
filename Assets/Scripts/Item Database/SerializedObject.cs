using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedObject : MonoBehaviour
{
    public Transform serializedTransform;

    public int m_itemID; //In ItemDB
    public int m_age; //In Days

    //Transform
    public float x, y, z;
    public float rx, ry, rz;
    public float sx, sy, sz;

    public void Update()
    {
        x = serializedTransform.position.x;
        y = serializedTransform.position.y;
        z = serializedTransform.position.z;

        rx = serializedTransform.rotation.eulerAngles.x;
        ry = serializedTransform.rotation.eulerAngles.y;
        rz = serializedTransform.rotation.eulerAngles.z;

        sx = serializedTransform.localScale.x;
        sy = serializedTransform.localScale.y;
        sz = serializedTransform.localScale.z;
    }
}
