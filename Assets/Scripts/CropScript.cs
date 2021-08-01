using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropScript : MonoBehaviour
{
    [Header("InputInfo")]
    public int m_growthPeriod;
    public GameObject m_plant;
    public Vector3 m_finalSize;
    private Vector3 m_stepSize;

    [Header("ReadOnly")]
    public int m_birthDay = -1;
    public int m_nextHarvest = 0;

    private int m_lastRecordedDay = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        SerializedObject data = GetComponentInParent<SerializedObject>();

        if(data)
        {
            transform.position = data.serializedTransform.position;
            transform.rotation = data.serializedTransform.rotation;
            transform.localScale = data.serializedTransform.localScale;

            m_birthDay = data.m_age;
        }
    }

    void Start()
    {
        m_birthDay = (m_birthDay == -1) ? GameManager.instance.m_day : m_birthDay;

        if(m_birthDay != GameManager.instance.m_day)
        {
            int diff = GameManager.instance.m_day - m_birthDay;
        }

        m_nextHarvest = m_birthDay + m_growthPeriod;

        m_stepSize = m_finalSize / m_growthPeriod;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_lastRecordedDay < GameManager.instance.m_day)
        {
            m_lastRecordedDay = GameManager.instance.m_day;
            StartCoroutine(GrowStep(m_plant.transform.localScale + m_stepSize, 1.0f));
        }
    }

    public void Harvest()
    {
        if(m_nextHarvest <= GameManager.instance.m_day)
        {
            //Harvested;
        }
    }

    public IEnumerator GrowStep(Vector3 target, float seconds)
    {
        Vector3 start = m_plant.transform.localScale;
        
        float time = 0.0f;
        while(time != seconds)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            m_plant.transform.localScale = Vector3.Lerp(start, target, time/seconds);
        }
        m_plant.transform.localScale = target;
        yield return null;
    }
}
