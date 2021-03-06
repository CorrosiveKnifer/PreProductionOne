using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    public bool m_spawnsWithForce = false;
    private ItemObject m_heldItem;
    private bool m_spawning = true;
    private float m_targetScale;
    private PlayerInventory m_targetPlayer;
    private float m_maxTime = 1.5f;
    public static void CreateLoot(int _id, uint _amount, Vector3 _position)
    {
        GameObject newDrop = Instantiate(Resources.Load<GameObject>("Prefabs/LootDrop"), _position, Quaternion.Euler(new Vector3(0, Random.Range(0.0f, 360.0f), 0)));
        newDrop.GetComponent<LootDrop>().SetHeldItem(_id, _amount);
    }

    public static void CreateLoot(int _id, uint _amount, Vector3 _position, Vector3 _force)
    {
        GameObject newDrop = Instantiate(Resources.Load<GameObject>("Prefabs/LootDrop"), _position, Quaternion.Euler(new Vector3(0, Random.Range(0.0f, 360.0f), 0)));
        newDrop.GetComponent<LootDrop>().m_spawnsWithForce = true;
        newDrop.GetComponent<LootDrop>().SetHeldItem(_id, _amount);
        newDrop.GetComponent<Rigidbody>().AddForce(_force);
    }

    private void Awake()
    {
        Physics.IgnoreLayerCollision(10, 11, true);
        Physics.IgnoreLayerCollision(10, 10, true);
        Physics.IgnoreLayerCollision(10, 9, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = !m_spawnsWithForce;
        m_targetPlayer = FindObjectOfType<PlayerInventory>();
        StartCoroutine(Spawning());
        m_targetScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!m_spawning)
        {
            m_maxTime -= Time.deltaTime;
            if(m_maxTime <= 0)
            {
                CollectLoot();
                return;
            }

            float newScale;
            float distance = Vector3.Distance(m_targetPlayer.transform.position, transform.position);
            if (distance < 2.0f)
            {
                if (distance < 0.5f)
                {
                    CollectLoot();
                    return;
                }
                newScale = Mathf.Lerp(m_targetScale, 0.0f, 1.0f - (distance / 2.0f));
                GetComponent<Rigidbody>().AddForce((m_targetPlayer.transform.position - transform.position).normalized * 0.5f);
                GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                newScale = 1.0f;
                GetComponent<Rigidbody>().useGravity = true;
            }
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * newScale;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f) * m_targetScale, 1 - Mathf.Pow(2.0f, -Time.deltaTime * 5.0f));
        }
    }

    IEnumerator Spawning()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        m_spawning = false;
        GetComponent<Rigidbody>().isKinematic = false;

    }

    private void CollectLoot()
    {
        m_targetPlayer.AddItem(m_heldItem);

        Destroy(gameObject);
    }

    public void SetHeldItem(int _id, uint _amount)
    {
        m_heldItem = ItemObject.CreateItem(_id, _amount);
    }
    
    public ItemObject GetItem()
    {
        return m_heldItem;
    }
}
