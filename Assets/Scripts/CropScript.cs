using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropScript : MonoBehaviour
{
    [Header("InputInfo")]
    public CropData m_data;
    public GameObject m_plant;
    public GameObject m_harvest;
    private Vector3 m_stepSize;
    

    [Header("ReadOnly")]
    public int m_birthDay = -1;
    private int m_nextHarvest = 0;
    public float m_waterValue = 0.0f;
    private float m_age = 1.0f;
    private int m_lastRecordedDay = 0;
    private Vector3 m_thisMaxHeight;

    // Start is called before the first frame update
    private void Awake()
    {
        m_harvest.GetComponent<Animator>().SetBool("IsHarvested", true);

        //Adjusting for variance
        m_thisMaxHeight = m_data.m_maxHeight * Random.Range(m_data.m_minVariance, m_data.m_maxVariance);
    }

    void Start()
    {
        if (GetComponentInParent<SerializedObject>() != null)
        {
            SerializedData data = GetComponentInParent<SerializedObject>().data;

            m_plant.transform.localScale = new Vector3(data.cx, data.cy, data.cz);

            m_nextHarvest = data.m_nextHarvest;

            m_age = data.m_age;
            m_waterValue = data.m_water;
        }
        else
        {
            m_nextHarvest = m_data.m_timeRequired;
        }

        m_stepSize = m_thisMaxHeight / m_data.m_maxAge;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_lastRecordedDay < GameManager.instance.m_day)
        {
            Grow(GameManager.instance.m_day - m_lastRecordedDay);
            m_lastRecordedDay = GameManager.instance.m_day;
            if(m_age >= m_data.m_maxAge * 0.95f)
            {
                m_nextHarvest = Mathf.Clamp(m_nextHarvest - 1, 0, m_data.m_timeRequired);
            }
            else
            {
                if (m_nextHarvest != 0 || m_age <= m_data.m_maxAge * 0.5f)
                    m_nextHarvest = m_data.m_timeRequired;
            }
        }

        GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, new Color(0.5f, 0.25f, 0), m_waterValue);

        m_harvest.GetComponent<Animator>().SetBool("IsHarvested", m_nextHarvest != 0);

        GetComponent<SerializedObject>().data.m_age = m_age;
        GetComponent<SerializedObject>().data.m_water = m_waterValue;
        GetComponent<SerializedObject>().data.m_nextHarvest = m_nextHarvest;
    }

    public void Interact()
    {
        ItemObject item = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().GetSelectItem();

        if(item.GetToolType() == ToolType.WaterCan)
        {
            float amount = Mathf.Clamp(item.m_amount-1, 0, 20.0f);
            Water(amount);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().RemoveItem(7, (int)amount);
            return;
        }

        if (item.GetToolType() == ToolType.Shovel)
        {
            Destroy(gameObject);
            return;
        }

        if (m_nextHarvest <= 0)
        {
            for (int i = 0; i < m_data.m_drops.Length; i++)
            {
                int amount = (m_data.m_drops[i].m_isRandom) ? Random.Range(m_data.m_drops[i].m_minDropAmount, m_data.m_drops[i].m_maxDropAmount) : m_data.m_drops[i].m_minDropAmount;
                LootDrop.CreateLoot(m_data.m_drops[i].m_itemDropID, (uint)amount, transform.position + transform.up);
            }

            m_nextHarvest += m_data.m_timeRequired;

            if (!m_data.m_isRenewable)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Grow(int daysPassed)
    {
        float growthRate = 0.0f;

        if(m_waterValue >= m_data.m_waterRequirement)
        {
            growthRate = m_waterValue;
        }
        else
        {
            growthRate = (m_data.m_waterRequirement - m_waterValue) / m_data.m_waterRequirement;
        }
       
        m_age = Mathf.Clamp(m_age + daysPassed * growthRate, 0.0f, m_data.m_maxAge);
        m_waterValue = Mathf.Clamp(m_waterValue - m_data.m_waterDecay, 0.0f, 1.0f);

        StartCoroutine(GrowStep(m_plant, m_plant.transform.localScale + m_stepSize * m_age, 1.0f));
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

    public void Water(float val)
    {
        m_waterValue = Mathf.Clamp(m_waterValue+(val/20.0f), 0.0f, 1.0f);
    }
}
