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
        m_slimeModel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * (1.0f + (m_size - 1.0f) * 0.25f);

        Color slimeColor;

        switch (m_size)
        {
            default:
                slimeColor = new Color(0.812f, 0.886f, 0.953f, 0.9f);
                break;
            case 2:
                slimeColor = new Color(0.427f, 0.620f, 0.922f, 0.9f);
                break;
            case 3:
                slimeColor = new Color(0.557f, 0.486f, 0.765f, 0.9f);
                break;
            case 4:
                slimeColor = new Color(0.651f, 0.302f, 0.475f, 0.9f);
                break;
            case 5:
                slimeColor = new Color(0.945f, 0.761f, 0.196f, 0.9f);
                break;
        }

        m_slimeModel.GetComponent<MeshRenderer>().material.color = slimeColor;
        m_health = 5 * m_size;
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
            Destroy(this.gameObject);
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
                // Create greater slime.
                GameObject newSlime = Instantiate(Resources.Load<GameObject>("Prefabs/Slime"), 
                    (transform.position + collision.transform.position) / 2.0f , transform.rotation);
                newSlime.GetComponent<Slime>().SetSize(m_size + 1);

                // Destroy other slime.
                Destroy(collision.gameObject);

                // Destroy this slime.
                Destroy(gameObject);
            }
        }
    }
}
