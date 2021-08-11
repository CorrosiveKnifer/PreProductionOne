using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public List<GameObject> m_obstructions;

    // Start is called before the first frame update
    void Awake()
    {
        m_obstructions = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != 8 && !other.isTrigger)
        {
            m_obstructions.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer != 8 && m_obstructions.Contains(other.gameObject))
        {
            m_obstructions.Remove(other.gameObject);
        }
    }

    public bool HasObstructions()
    {
        return m_obstructions.Count != 0;
    }
}
