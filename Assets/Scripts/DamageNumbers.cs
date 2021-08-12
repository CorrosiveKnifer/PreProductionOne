using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumbers : MonoBehaviour
{
    private Camera m_camera;
    public int m_value;
    public Vector3 m_position;
    public Text m_text;
    private float m_timeAlive = 0.0f;
    private float m_maxDuration = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        m_timeAlive += Time.deltaTime;
        m_text.transform.position = m_camera.WorldToScreenPoint(m_position + Vector3.up * (1.0f + m_timeAlive));
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b,
            (m_maxDuration - m_timeAlive) / m_maxDuration);
        if (m_timeAlive >= m_maxDuration)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateDamageValue(int _value)
    {
        m_value = _value;
        m_text.text = _value.ToString();
    }
    public void SetPosition(Vector3 _position)
    {
        m_position = _position;
    }
}
