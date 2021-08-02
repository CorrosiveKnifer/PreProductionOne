using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityForceBehavour : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera m_cam;

    public List<GameObject> m_influenced = new List<GameObject>();

    void Start()
    {
        m_cam = GetComponent<Camera>();    
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
        Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;

        RaycastHit[] hits = Physics.RaycastAll(m_cam.transform.position, direction, distance);

        List<GameObject> nextList = new List<GameObject>();
        foreach (var item in hits)
        {
            nextList.Add(item.collider.gameObject);
            foreach (var renderer in item.collider.gameObject.GetComponentsInParent<MeshRenderer>())
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.5f);
            }
            foreach (var renderer in item.collider.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.5f);
            }
        }

        foreach (var item in m_influenced)
        {
            if (!nextList.Contains(item))
            {
                foreach (var renderer in item.gameObject.GetComponentsInParent<MeshRenderer>())
                {
                    renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f);
                }
                foreach (var renderer in item.gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f);
                }
            }
        }
        m_influenced = nextList;
    }
}
