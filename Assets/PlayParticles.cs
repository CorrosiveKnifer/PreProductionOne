using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticles : StateMachineBehaviour
{
    public float m_seconds;
    public bool m_isForever = false;
    private float m_currentTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInChildren<ParticleSystem>().Play();
        m_currentTime = m_seconds;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!m_isForever)
        {
            if (m_currentTime > 0)
            {
                m_currentTime -= Time.deltaTime;
            }
            else if (animator.GetComponentInChildren<ParticleSystem>().isPlaying)
            {
                animator.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponentInChildren<ParticleSystem>().isPlaying)
        {
            animator.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
