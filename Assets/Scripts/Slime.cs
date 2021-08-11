using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    public GameObject m_target;
    public GameObject m_slimeModel;

    private EnemyHealthbar m_healthbar;

    private bool m_combining = false;
    private bool m_spawning = true;
    private bool m_dead = false;

    private int m_size = 1;
    private int m_health = 5;
    private int m_maxHealth = 5;

    private float m_knockbackTimer = 0.0f;
    private float m_knockbackDuration = 1.0f;

    private float m_attackTimer = 0.0f;
    private float m_attackCooldown = 1.0f;

    private void Awake()
    {
        m_healthbar = GetComponentInChildren<EnemyHealthbar>();
        m_healthbar.transform.position = Camera.main.WorldToScreenPoint(transform.position + transform.up);
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_target = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_spawning)
            m_healthbar.HideHealth(m_combining);

        if (m_combining && !m_dead)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1 - Mathf.Pow(2.0f, -Time.deltaTime * 10.0f));
            if (transform.localScale.x < 0.2f)
            {
                Destroy(gameObject);
            }
        }
        else if (m_spawning)
        {
            float targetScale = 1.0f + (m_size - 1.0f) * 0.75f;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f) * targetScale, 1 - Mathf.Pow(2.0f, -Time.deltaTime * 5.0f));
            if (Mathf.Abs(targetScale - transform.localScale.x) < 0.2f)
            {
                m_spawning = false;
            }
        }

        if (m_knockbackTimer >= 0.0f)
        {
            m_knockbackTimer -= Time.deltaTime;
        }
        else if (!m_combining && !m_spawning && !m_dead)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<NavMeshAgent>().destination = m_target.transform.position;

            if (m_attackTimer > 0.0f)
            {
                m_attackTimer -= Time.deltaTime;
            }
        }
        m_healthbar.SetHealthPercentage((float)m_health / (float)m_maxHealth);
        m_healthbar.transform.position = Camera.main.WorldToScreenPoint(transform.position + transform.up);
    }


    public void SetSize(int _size)
    {
        m_size = _size;

        SetColorViaSize(_size);

        m_health = 5 * m_size;
        m_maxHealth = m_health;
    }

    public int GetSize()
    {
        return m_size;
    }

    public void SetColorViaSize(int _size)
    {
        float alpha = m_slimeModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color.a;
        Color slimeColor;
        switch (_size)
        {
            default:
                slimeColor = new Color(0.812f, 0.886f, 0.953f, alpha);
                break;
            case 2:
                slimeColor = new Color(0.427f, 0.620f, 0.922f, alpha);
                break;
            case 3:
                slimeColor = new Color(0.557f, 0.486f, 0.765f, alpha);
                break;
            case 4:
                slimeColor = new Color(0.651f, 0.302f, 0.475f, alpha);
                break;
            case 5:
                slimeColor = new Color(0.945f, 0.761f, 0.196f, alpha);
                break;
        }
        m_slimeModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = slimeColor;
    }


    public void Knockback(Vector3 _dir, float _power)
    {
        // Disable nav mesh
        GetComponent<NavMeshAgent>().enabled = false;

        // Apply knockback
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(_dir * _power, ForceMode.Impulse);

        // Start checking for collision detection with another slime
        // for a short duration after knockback is applied.
        m_knockbackTimer = m_knockbackDuration;
    }

    public void DamageEnemy(int _damage)
    {
        StartCoroutine(DamageFlash());

        m_health -= _damage;

        if (m_health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    IEnumerator DamageFlash()
    {
        m_slimeModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color(1.0f, 0.2f, 0.2f, 1.0f);
        yield return new WaitForSecondsRealtime(0.05f);
        SetColorViaSize(m_size);
    }
    IEnumerator Death()
    {
        m_dead = true;
        GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSecondsRealtime(2.0f);
        DropLoot();
        Destroy(gameObject);
    }
    private void DropLoot()
    {
        LootDrop.CreateLoot(0, (uint)m_size, transform.position + transform.up);
    }

    private void Attack()
    {
        m_target.GetComponent<PlayerVitality>().Damage(m_size * 5.0f);
        m_attackTimer = m_attackCooldown;
        GetComponentInChildren<Animator>().SetTrigger("Attack");
    }

    public void SetToCombine()
    {
        m_combining = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<NavMeshAgent>().enabled = false;
    }
    public bool GetCombining()
    {
        return m_combining;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_combining)
            return;

        if (collision.gameObject.GetComponent<Slime>() && m_knockbackTimer >= 0.0f)
        {
            if (collision.gameObject.GetComponent<Slime>().GetSize() == m_size && !collision.gameObject.GetComponent<Slime>().GetCombining())
            {
                // Create greater slime.
                GameObject newSlime = Instantiate(Resources.Load<GameObject>("Prefabs/Slime"), 
                    (transform.position + collision.transform.position) / 2.0f , transform.rotation);
                newSlime.GetComponent<Slime>().SetSize(m_size + 1);

                // Destroy other slime.
                collision.gameObject.GetComponent<Slime>().SetToCombine();

                // Destroy this slime.
                SetToCombine();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() && m_attackTimer <= 0.0f && !m_dead)
        {
            Attack();
        }
    }
}
