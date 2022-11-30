using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    PlayerController m_PlayerController;
    public float m_StartPctTime;
    public float m_EndPctTime;
    public TPunchType m_PunchType;
    private bool m_PunchActive;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerController = animator.GetComponent<PlayerController>();
        m_PlayerController.SetPunchActive(m_PunchType, false);
        m_PunchActive = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_PunchActive && stateInfo.normalizedTime >= m_StartPctTime && stateInfo.normalizedTime <= m_EndPctTime)
        {
            m_PlayerController.SetPunchActive(m_PunchType, true);
            m_PunchActive = true;
        }

        else if (m_PunchActive && stateInfo.normalizedTime > m_EndPctTime)
        {
            m_PlayerController.SetPunchActive(m_PunchType, false);
            m_PunchActive = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerController.SetPunchActive(m_PunchType, false);
        m_PlayerController.SetIsPunchEnable(false);
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
