using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedObject : MonoBehaviour
{
    public int m_itemID; //In ItemDB
    public int m_age = -1; //In Days

    public float cx, cy, cz;
    //Transform
    public float x, y, z;
    public float rx, ry, rz;

    public void Update()
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;

        rx = transform.rotation.eulerAngles.x;
        ry = transform.rotation.eulerAngles.y;
        rz = transform.rotation.eulerAngles.z;

        cx = GetComponentInChildren<CropScript>().m_plant.transform.localScale.x;
        cy = GetComponentInChildren<CropScript>().m_plant.transform.localScale.y;
        cz = GetComponentInChildren<CropScript>().m_plant.transform.localScale.z;
    }

    internal void UpdateTo(SerializedObject item)
    {
        m_itemID = item.m_itemID;
        m_age = item.m_age;

        x = item.x;
        y = item.y;
        z = item.z;

        rx = item.x;
        ry = item.y;
        rz = item.z;

        cx = item.cx;
        cy = item.cy;
        cz = item.cz;
    }
}
