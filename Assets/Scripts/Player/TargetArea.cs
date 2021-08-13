using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    Struct

    public List<GameObject> m_obstructions;

    // Start is called before the first frame update
    void Awake()
    {
        m_obstructions = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var obstruction in m_obstructions)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("Ground")  
            && other.transform.gameObject.layer != 2 && !other.isTrigger && !m_obstructions.Contains(other.gameObject))
        {
            m_obstructions.Add(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_obstructions.Contains(other.gameObject))
        {
            m_obstructions.Remove(other.gameObject);
        }
    }

    public bool HasObstructions()
    {
        return m_obstructions.Count != 0;
    }
}
