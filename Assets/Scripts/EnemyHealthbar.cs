using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    public Image m_healthFill;
    public Image m_healthDamageBack;
    public Image m_healthBackground;
    private float m_barChaseRate = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        HideHealth(true);
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        m_healthDamageBack.fillAmount -= m_barChaseRate * Time.deltaTime;
        if (m_healthDamageBack.fillAmount < m_healthFill.fillAmount)
            m_healthDamageBack.fillAmount = m_healthFill.fillAmount;


        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 1 - Mathf.Pow(2.0f, -Time.deltaTime * 5.0f));
    }

    public void SetHealthPercentage(float _percentage)
    {
        m_healthFill.fillAmount = _percentage;
    }

    public void HideHealth(bool _hidden)
    {
        m_healthFill.enabled = !_hidden;
        m_healthDamageBack.enabled = !_hidden;
        m_healthBackground.enabled = !_hidden;
        GetComponent<Image>().enabled = !_hidden;
    }
}
