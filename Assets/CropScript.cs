using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropScript : MonoBehaviour
{
    [Header("InputInfo")]
    public int m_growthPeriod;

    [Header("ReadOnly")]
    public int m_birthDay = 0;
    public int m_nextHarvest = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_birthDay = GameManager.instance.m_day;
        m_nextHarvest = m_birthDay + m_growthPeriod;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Harvest()
    {
        if(m_nextHarvest <= GameManager.instance.m_day)
        {
            //Harvested;
        }
    }
}
