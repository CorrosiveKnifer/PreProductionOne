using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedData
{
    public int m_itemID; //In ItemDB
    public float m_age = 0; //In Days
    public int m_nextHarvest = -1;
    public float m_water;

    public float cx, cy, cz;
    //Transform
    public float x, y, z;
    public float rx, ry, rz;
}

public class SerializedObject : MonoBehaviour
{
    public SerializedData data;

    public void Update()
    {
        data.x = transform.position.x;
        data.y = transform.position.y;
        data.z = transform.position.z;
        
        data.rx = transform.rotation.eulerAngles.x;
        data.ry = transform.rotation.eulerAngles.y;
        data.rz = transform.rotation.eulerAngles.z;
        
        data.cx = GetComponentInChildren<CropScript>().m_plant.transform.localScale.x;
        data.cy = GetComponentInChildren<CropScript>().m_plant.transform.localScale.y;
        data.cz = GetComponentInChildren<CropScript>().m_plant.transform.localScale.z;
    }

    public void UpdateTo(SerializedData item)
    {
        data = new SerializedData();
        data.m_itemID = item.m_itemID;
        data.m_age = item.m_age;
        data.m_nextHarvest = item.m_nextHarvest;
        data.m_water = item.m_water;

        data.x = item.x;
        data.y = item.y;
        data.z = item.z;
        
        data.rx = item.x;
        data.ry = item.y;
        data.rz = item.z;
        
        data.cx = item.cx;
        data.cy = item.cy;
        data.cz = item.cz;
    }
}
