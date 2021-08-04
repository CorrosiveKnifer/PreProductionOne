using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    public GameObject m_slimeModel;
    private int m_size = 1;
    private int m_health = 5;

    private float m_knockbackTimer = 0.0f;
    private float m_knockbackDuration = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(m_knockbackTimer >= 0.0f)
        {
            m_knockbackTimer -= Time.deltaTime;
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

    public void SetSize(int _size)
    {
        m_size = _size;
    }

    public int GetSize()
    {
        return m_size;
    }

    public void Knockback(Vector3 _dir, float _power)
    {
        // Disable nav mesh
        GetComponent<NavMeshAgent>().enabled = false;

        // Apply knockback
        GetComponent<Rigidbody>().AddForce(_dir * _power, ForceMode.Impulse);

        // Start checking for collision detection with another slime
        // for a short duration after knockback is applied.
        m_knockbackTimer = m_knockbackDuration;
    }

    public void DamageEnemy(int _damage)
    {
        m_health -= _damage;
        if (m_health <= 0)
        {
            DropLoot();
            Destroy(this);
        }
    }

    private void DropLoot()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Slime>() && m_knockbackTimer >= 0.0f)
        {
            if (collision.gameObject.GetComponent<Slime>().GetSize() == m_size)
            {
                // Destroy other slime.
                Destroy(collision.gameObject);

                // Create greater slime.


                // Destroy this slime.
                Destroy(this);
            }
        }
    }
}
