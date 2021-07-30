using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    private Light m_directional;

    public float m_secondsPerDay;
    public float m_hourOfRise;
    public float m_hourOfNoon;
    public float m_hourOfSet;

    [Header("Stats")]
    public float m_currentHour;
    // Start is called before the first frame update
    void Start()
    {
        m_directional = GetComponent<Light>();
        m_currentHour = m_hourOfRise - 0.10f;
    }

    // Update is called once per frame
    void Update()
    {
        float secondsPerHour = (m_secondsPerDay / 24.0f); //1minute for 1 ingame day
        float secondsPassed = Time.deltaTime;

        m_currentHour += secondsPassed / secondsPerHour;

        if (m_currentHour > 23)
            m_currentHour -= 23;

        float rotationVal = 0;
        if (m_currentHour >= m_hourOfRise && m_currentHour < m_hourOfNoon)
        {
            rotationVal = Mathf.Lerp(0, 90, (m_currentHour - m_hourOfRise) / (m_hourOfNoon - m_hourOfRise));
        }
        else if(m_currentHour >= m_hourOfNoon && m_currentHour < m_hourOfSet)
        {
            rotationVal = Mathf.Lerp(90, 180, (m_currentHour - m_hourOfNoon) / (m_hourOfSet - m_hourOfNoon));
        }
        else if(m_currentHour >= m_hourOfSet && m_currentHour < 23)
        {
            rotationVal = Mathf.Lerp(180, 270, (m_currentHour - m_hourOfSet) / (23 - m_hourOfSet));
        }
        else
        {
            rotationVal = Mathf.Lerp(270, 360, (m_currentHour - 0) / (m_hourOfRise - 0));
        }
        
        transform.rotation = Quaternion.Euler(new Vector3(rotationVal, 0, 0));
    }
}
