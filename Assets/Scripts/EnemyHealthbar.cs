using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    public Image m_healthFill;
    public Image m_healthDamageBack;
    private float m_barChaseRate = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_healthDamageBack.fillAmount -= m_barChaseRate * Time.deltaTime;
        if (m_healthDamageBack.fillAmount < m_healthFill.fillAmount)
            m_healthDamageBack.fillAmount = m_healthFill.fillAmount;
    }

    public void SetHealthPercentage(float _percentage)
    {
        m_healthFill.fillAmount = _percentage;
    }
}
