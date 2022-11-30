using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoombaSMC : MonoBehaviour
{

    public enum IState
    {
        PATROL,
        ALERT,
        ATTACK,
        DIE
    }
    [Header("References")]
    Animator m_Animator;

    [Header("Patrol")]
    public IState m_State;
    NavMeshAgent m_NavMeshAgent;
    public List<Transform> m_PatrolTargets;
    int m_CurrentPatrolTargetID = 0;
    public float m_PatrolSpeed = 3.5f;
    public float m_RotationSpeed = 10f;

    [Header("Alert")]
    public float m_ConeVisualAngle;
    public float m_SightDistance = 5.0f;
    public LayerMask m_SightLayer;
    public float m_EyesHeight = 1f;


    [Header("Attack")]
    public float m_AttackSpeed = 5f;
    public float m_TimeChasing = 3f;
    float m_CurrentTimeChasing;
    public float m_RecoveryTime = 2.0f;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        SetPatrolState();
    }


    void Update()
    {
        switch (m_State)
        {
            case IState.PATROL:
                UpdatePatrolState();
                break;
                
            case IState.ATTACK:
                UpdateAttackState();
                break;
            case IState.DIE:
                UpdateDieState();
                break;

        }



    }


    void SetPatrolState()
    {  
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = m_PatrolSpeed;
        m_Animator.SetFloat("Speed", 0.1f);
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetID].position;
        m_State = IState.PATROL;
    }

    bool PatrolTargetPositionArrived()
    {
        return !m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
    }

    void MoveToNextPatrolPosition()
    {
        ++m_CurrentPatrolTargetID;
        if (m_CurrentPatrolTargetID >= m_PatrolTargets.Count)
            m_CurrentPatrolTargetID = 0;
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetID].position;
    }
    void UpdatePatrolState()
    {
        if (PatrolTargetPositionArrived())
        {
            MoveToNextPatrolPosition();
        }
        if (SeePlayer() && m_State != IState.ALERT)
        {
            SetAttackState();
        }
    }

    bool SeePlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_DirectionToPlayerXZ = l_PlayerPosition - transform.position;
        l_DirectionToPlayerXZ.y = 0.0f;
        l_DirectionToPlayerXZ.Normalize();

        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.y = 0.0f;
        l_ForwardXZ.Normalize();

        Vector3 l_EyesPosition = transform.position + Vector3.up * m_EyesHeight;
        Vector3 l_PlayerEyesPosition = l_PlayerPosition;
        Vector3 l_Direction = l_PlayerEyesPosition - l_EyesPosition;

        float l_Length = l_Direction.magnitude;
        l_Direction /= l_Length;
        Ray l_Ray = new Ray(l_EyesPosition, l_PlayerEyesPosition);


        return Vector3.Distance(l_PlayerPosition, transform.position) < m_SightDistance
            && Vector3.Dot(l_ForwardXZ, l_DirectionToPlayerXZ) > Mathf.Cos(m_ConeVisualAngle * Mathf.Deg2Rad / 2.0f)
            && !Physics.Raycast(l_Ray, l_Length, m_SightLayer);
    }


   
    IEnumerator Recovery()
    {
        m_Animator.Play("Idle");
        m_NavMeshAgent.isStopped = true;
        yield return new WaitForSeconds(2.5f);
        SetPatrolState();
    }
    void SetAttackState()
    {
        m_Animator.SetTrigger("Alert");
        m_Animator.SetFloat("Speed", 1);
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = m_AttackSpeed;
        
        m_State = IState.ATTACK;
    }
    void UpdateAttackState()
    {
        m_NavMeshAgent.destination = GameController.GetGameController().GetPlayer().transform.position;
        m_CurrentTimeChasing += Time.deltaTime;
        if (m_CurrentTimeChasing >= m_TimeChasing)
        {
            m_CurrentTimeChasing = 0f;
            StartCoroutine(Recovery());
            
        }

    }

    void SetDieState()
    {

    }
    void UpdateDieState()
    {

    }
}
