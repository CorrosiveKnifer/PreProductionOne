using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedObject : MonoBehaviour
{
    public int m_itemID; //In ItemDB
    public int m_age; //In Days

    //Transform
    public float x, y, z;
    public float rx, ry, rz;
    public float sx, sy, sz;

    public void Update()
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;

        rx = transform.rotation.eulerAngles.x;
        ry = transform.rotation.eulerAngles.y;
        rz = transform.rotation.eulerAngles.z;

        sx = transform.localScale.x;
        sy = transform.localScale.y;
        sz = transform.localScale.z;
    }
}
