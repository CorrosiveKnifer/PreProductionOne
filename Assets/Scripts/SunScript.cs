using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public float m_secondsPerDay;
    public float m_hourOfRise = 6;
    public float m_hourOfNoon = 12;
    public float m_hourOfSet = 20;

    // Update is called once per frame
    void Update()
    {
        float secondsPerHour = (m_secondsPerDay / 24.0f); //1 minute for 1 ingame day
        float secondsPassed = Time.deltaTime;

        GameManager.instance.SkipTime(secondsPassed / secondsPerHour);

        float hour = GameManager.instance.m_currentHour;

        float rotationVal = 0;
        if (hour >= m_hourOfRise && hour < m_hourOfNoon)
        {
            rotationVal = Mathf.Lerp(0, 90, (hour - m_hourOfRise) / (m_hourOfNoon - m_hourOfRise));
        }
        else if(hour >= m_hourOfNoon && hour < m_hourOfSet)
        {
            rotationVal = Mathf.Lerp(90, 180, (hour - m_hourOfNoon) / (m_hourOfSet - m_hourOfNoon));
        }
        else if(hour >= m_hourOfSet && hour < 23)
        {
            rotationVal = Mathf.Lerp(180, 270, (hour - m_hourOfSet) / (23 - m_hourOfSet));
        }
        else
        {
            rotationVal = Mathf.Lerp(270, 360, (hour - 0) / (m_hourOfRise - 0));
        }
        
        transform.rotation = Quaternion.Euler(new Vector3(rotationVal, 0, 0));
    }
    public float GetTimePassed()
    {
        return (m_secondsPerDay / 24.0f) / Time.deltaTime;
    }
}
