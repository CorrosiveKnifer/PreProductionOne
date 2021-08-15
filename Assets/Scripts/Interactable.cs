using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent m_interactFunction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInteractor>())
        {
            other.GetComponent<PlayerInteractor>().AddCollider(transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInteractor>())
        {
            other.GetComponent<PlayerInteractor>().RemoveCollider(transform.parent.gameObject);
        }
    }

    public void Interact()
    {
        m_interactFunction.Invoke();
    }

    public void TestInteraction()
    {
        Debug.Log("Testing... Testing... Is this thing on? Hello?");
    }
}
