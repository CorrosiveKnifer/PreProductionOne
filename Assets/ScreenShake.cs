using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Animator m_animator;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void StartScreenShake()
    {
        m_animator.SetTrigger("Start");
    }
}
