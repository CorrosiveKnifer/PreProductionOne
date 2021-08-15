using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public float m_secondsPerDay;
    public bool m_isRaining = false;
    public float m_weatherTimer = 0.0f;
    [Range(0.0f, 1.0f)]
    public float m_rainPercentage;
    public float m_weatherCheckInHours = 8.0f;

    [Header("Sun")]
    public Light m_sun;
    public Gradient m_sunColor;
    public AnimationCurve m_sunIntensity;

    [Header("Moon")]
    public Light m_moon;
    public Gradient m_moonColor;
    public AnimationCurve m_moonIntensity;

    [Header("Other")]
    public AnimationCurve m_lightingIntensity;
    public AnimationCurve m_reflectionIntensity;

    [Header("Weather")]
    public GameObject m_rain;

    private float m_currentHour;
    private int m_currentDay;
    private void Start()
    {
        m_currentHour = GameManager.instance.m_currentHour;
        m_currentDay = GameManager.instance.m_day;

        if(m_isRaining)
        {
            m_rain.GetComponent<SoloAudioAgent>().PlayWithFadeIn(0.5f);
        }
        else
        {
            foreach (var system in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = system.emission;
                emission.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float secondsPerHour = (m_secondsPerDay / 24.0f); //1 minute for 1 ingame day
        float secondsPassed = Time.deltaTime;

        GameManager.instance.SkipTime(secondsPassed / secondsPerHour);

        if (m_weatherTimer > 0)
            m_weatherTimer -= GetInGameDeltaHours();
        else
        {
            bool rainBefore = m_isRaining;
            m_isRaining = Random.Range(0, 1000) > (1.0 - m_rainPercentage) * 1000; //60% clear weather;
            m_weatherTimer += m_weatherCheckInHours; //In game Hours

            if(m_isRaining && !rainBefore)
            {
                m_rain.GetComponent<SoloAudioAgent>().PlayWithFadeIn(1.0f);
                foreach (var system in GetComponentsInChildren<ParticleSystem>())
                {
                    var emission = system.emission;
                    emission.enabled = true;
                }
            }
            if (!m_isRaining && rainBefore)
            {
                m_rain.GetComponent<SoloAudioAgent>().PauseWithFadeOut(1.0f);
                foreach (var system in GetComponentsInChildren<ParticleSystem>())
                {
                    var emission = system.emission;
                    emission.enabled = false;
                }
            }
        }

        Rotate();

        m_currentHour = GameManager.instance.m_currentHour;
        m_currentDay = GameManager.instance.m_day;
    }

    public void Rotate()
    {
        float val = GameManager.instance.m_currentHour / 24.0f;
        m_sun.transform.eulerAngles = (val - 0.25f) * new Vector3(90, 0, 0) * 4.0f;
        m_moon.transform.eulerAngles = (val - 0.75f) * new Vector3(90, 0, 0) * 4.0f;

        //Intensity
        m_sun.intensity = m_sunIntensity.Evaluate(val);
        m_moon.intensity = m_moonIntensity.Evaluate(val);

        //Color
        m_sun.color = m_sunColor.Evaluate(val);
        m_moon.color = m_moonColor.Evaluate(val);

        //Enable
        if(m_sun.intensity == 0 && m_sun.gameObject.activeInHierarchy)
            m_sun.gameObject.SetActive(false);
        else if(m_sun.intensity > 0 && !m_sun.gameObject.activeInHierarchy)
            m_sun.gameObject.SetActive(true);

        //Enable
        if (m_moon.intensity == 0 && m_moon.gameObject.activeInHierarchy)
            m_moon.gameObject.SetActive(false);
        else if (m_moon.intensity > 0 && !m_moon.gameObject.activeInHierarchy)
            m_moon.gameObject.SetActive(true);

        RenderSettings.ambientIntensity = m_lightingIntensity.Evaluate(val);
        RenderSettings.reflectionIntensity = m_reflectionIntensity.Evaluate(val);

        GetComponent<FadeDualAgent>().SetFadeValues(val, val);
    }
    private float GetInGameDeltaHours()
    {
        float oldHour = m_currentHour;
        float nextHour = GameManager.instance.m_currentHour;
        if (m_currentDay < GameManager.instance.m_day)
        {
            nextHour += 23.0f;
        }

        return nextHour - oldHour;
    }
}
