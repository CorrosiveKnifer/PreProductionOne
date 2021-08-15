using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXdie : MonoBehaviour
{
    public float m_waitTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Kill());
    }
    IEnumerator Kill()
    {
        yield return new WaitForSecondsRealtime(m_waitTime);
        Destroy(gameObject);
    }
}
