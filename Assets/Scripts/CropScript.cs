using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropScript : MonoBehaviour
{
    [Header("InputInfo")]
    public int m_growthPeriod;
    public GameObject m_plant;
    public GameObject m_harvest;
    public Vector3 m_finalSize;
    public int m_itemID = -1;
    public int m_dropAmount = 0;
    private Vector3 m_stepSize;

    [Header("ReadOnly")]
    public int m_birthDay = -1;
    public int m_nextHarvest = 0;

    private int m_age = 0;
    private int m_lastRecordedDay = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        m_harvest.GetComponent<Animator>().SetBool("IsHarvested", true);
    }

    void Start()
    {
        if (GetComponentInParent<SerializedObject>() != null)
        {
            SerializedData data = GetComponentInParent<SerializedObject>().data;

            m_plant.transform.localScale = new Vector3(data.cx, data.cy, data.cz);

            m_birthDay = (data.m_age == -1) ? GameManager.instance.m_day : data.m_age;

            m_nextHarvest = (data.m_age == -1) ? m_age + m_growthPeriod : data.m_nextHarvest;

            m_age = GameManager.instance.m_day - m_birthDay;
        }
        else
        {
            m_nextHarvest = m_age + m_growthPeriod;
        }

        m_stepSize = m_finalSize / m_growthPeriod;

        Grow();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_lastRecordedDay < GameManager.instance.m_day)
        {
            m_lastRecordedDay = GameManager.instance.m_day;
            Grow();
        }

        GetComponent<SerializedObject>().data.m_age = m_age;
        GetComponent<SerializedObject>().data.m_nextHarvest = m_nextHarvest;
    }

    public void Harvest()
    {
        if(m_nextHarvest <= GameManager.instance.m_day)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().AddItem(ItemObject.CreateItem(m_itemID, (uint)m_dropAmount));
            m_harvest.GetComponent<Animator>().SetBool("IsHarvested", true);

            m_nextHarvest += m_growthPeriod;
        }
    }

    public void Grow()
    {
        int diff = GameManager.instance.m_day - m_age;

        if (diff > 0 && m_plant.transform.localScale != m_finalSize)
        {
            if((m_plant.transform.localScale + m_stepSize * diff).magnitude >= m_finalSize.magnitude)
            {
                StartCoroutine(GrowStep(m_plant, m_finalSize, 1.0f));
            }
            else
            {
                StartCoroutine(GrowStep(m_plant, m_plant.transform.localScale + m_stepSize * diff, 1.0f));
            }
        }
        else
        {
            if (m_nextHarvest <= GameManager.instance.m_day && m_harvest.transform.position != new Vector3(1.0f, 1.0f, 1.0f))
            {
                m_harvest.GetComponent<Animator>().SetBool("IsHarvested", false);
            }
        }

        m_age += diff;
    }

    public IEnumerator GrowStep(GameObject objectToScale, Vector3 target, float seconds)
    {
        Vector3 start = objectToScale.transform.localScale;
        
        float time = 0.0f;
        while(time != seconds)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            objectToScale.transform.localScale = Vector3.Lerp(start, target, time/seconds);
        }
        objectToScale.transform.localScale = target;
        yield return null;
    }
}
