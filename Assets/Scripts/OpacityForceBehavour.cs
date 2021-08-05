using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityForceBehavour : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera m_cam;
    [Range(0.0f, 1.0f)]
    public float m_alphaVal = 0.5f;
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
            if(item.collider.tag == "Player")
            {
                continue;
            }

            nextList.Add(item.collider.gameObject);
            foreach (var renderer in item.collider.gameObject.GetComponentsInParent<MeshRenderer>())
            {
                renderer.material.SetFloat("_Surface", 1.0f);
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, m_alphaVal);
            }
            foreach (var renderer in item.collider.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material.SetFloat("_Surface", 1.0f);
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, m_alphaVal);
            }
        }

        foreach (var item in m_influenced)
        {
            if (!nextList.Contains(item))
            {
                foreach (var renderer in item.gameObject.GetComponentsInParent<MeshRenderer>())
                {
                    renderer.material.SetFloat("_Surface", 0.0f);
                    renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f);
                }
                foreach (var renderer in item.gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.material.SetFloat("_Surface", 0.0f);
                    renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f);
                }
            }
        }
        m_influenced = nextList;
    }
}
