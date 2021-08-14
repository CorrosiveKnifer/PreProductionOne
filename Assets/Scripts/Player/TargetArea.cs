using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{

    public Dictionary<GameObject, int> m_obstructions;

    // Start is called before the first frame update
    void Awake()
    {
        m_obstructions = new Dictionary<GameObject, int>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<GameObject> toIncrement = new List<GameObject>();
        foreach (var obstruction in m_obstructions)
        {
            toIncrement.Add(obstruction.Key);
        }
        foreach (var gameObject in toIncrement)
        {
            m_obstructions[gameObject] = m_obstructions[gameObject] + 1;
        }

        List<GameObject> toDestroy = new List<GameObject>();
        foreach (var obstruction in m_obstructions)
        {
            if ((obstruction.Value > 3) && m_obstructions.ContainsKey(obstruction.Key))
            {
                toDestroy.Add(obstruction.Key);
                break;
            }
        }

        foreach (var gameObject in toDestroy)
        {
            m_obstructions.Remove(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_obstructions.ContainsKey(other.gameObject))
        {
            m_obstructions[other.gameObject] = 0;
        }
    }
    private void LateUpdate()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("Ground")  
            && other.transform.gameObject.layer != 2 && !other.isTrigger && !m_obstructions.ContainsKey(other.gameObject))
        {
            m_obstructions.Add(other.gameObject, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_obstructions.ContainsKey(other.gameObject))
        {
            m_obstructions.Remove(other.gameObject);
        }
    }

    public bool HasObstructions()
    {
        return m_obstructions.Count != 0;
    }
}
