
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KoopaSMC : MonoBehaviour, IRestartGameElement
{

    public enum IState
    {
        PATROL,
        ALERT,
        ATTACK,
        HIT,

    }
    [Header("References")]
    float m_CurrentSpeed;
    PlayerController m_Player;
    CharacterController m_CharacterController;
    public GameObject m_KoopaShell;

    [Header("Patrol")]
    public IState m_State;
    NavMeshAgent m_NavMeshAgent;
    public List<Transform> m_PatrolTargets;
    int m_CurrentPatrolTargetID = 0;
    public float m_PatrolSpeed = 3.5f;
    public float m_RotationSpeed = 10f;

    [Header("Alert")]
    public LayerMask m_SightLayer;
    public float m_HearRange = 5f;
    [SerializeField] AnimationClip m_AlertAnimation;


    [Header("Attack")]
    public float m_AttackSpeed = 5f;
    public float m_TimeChasing = 3f;
    float m_CurrentTimeChasing;
    public float m_RecoveryTime = 2.0f;
    Vector3 m_TargetDirection;
    public float m_AttackingTimer = 2f;
    Vector3 m_Movement;

    [Header("Hit")]
    Vector3 m_HitDirection;
    public float m_HitSpeed = 80f;
    public float m_MaxHitTime = 025f;
    float m_CurrentHitTime;


    private void Awake()
    {
        
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
        
        GameController.GetGameController().AddRestartGameElement(this);
    }

    void Start()
    {

        m_Player = GameController.GetGameController().GetPlayer();
        SetPatrolState();
       

    }


    void Update()
    {
        switch (m_State)
        {
            case IState.PATROL:
                UpdatePatrolState();
                break;
            case IState.ALERT:
                UpdateAlertState();
                break;

            case IState.ATTACK:
                UpdateAttackState();
                break;
            case IState.HIT:
                UpdateHitState();
                break;

        }
    }

    void SetPatrolState()
    {
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = m_PatrolSpeed;
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
        if (HearsPlayer())
        {
            SetAlertState();
        }
    }

    bool HearsPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        return Vector3.Distance(l_PlayerPosition, transform.position) <= m_HearRange && GameController.GetGameController().GetPlayer().m_HasMovement;
    }

    void SetAttackState()
    {

        m_State = IState.ATTACK;
        m_NavMeshAgent.isStopped = true;
        m_Movement = m_Player.transform.position - transform.position;
        m_Movement.y = 0;
        m_Movement.Normalize();

    }
    void UpdateAttackState()
    {

        m_CharacterController.Move(m_Movement * m_AttackSpeed * Time.deltaTime);
        m_CurrentTimeChasing += Time.deltaTime;

        if (m_CurrentTimeChasing >= m_TimeChasing)
        {
            SetPatrolState();
            m_CurrentTimeChasing = 0;

        }

    }



    void SetAlertState()
    {
        m_NavMeshAgent.destination = m_Player.transform.position;
        m_State = IState.ALERT;
        m_NavMeshAgent.isStopped = true;


    }

    void UpdateAlertState()
    {
        SetAttackState();

    }

    public void SetHitState(Vector3 Direction)
    {
        m_State = IState.HIT;
        m_HitDirection = Direction;
        m_HitDirection.y = 0;
    }

    void UpdateHitState()
    {

        m_CharacterController.Move(m_HitDirection * m_HitSpeed * Time.deltaTime);
        m_CurrentHitTime += Time.deltaTime;

        if (m_CurrentHitTime >= m_MaxHitTime)
        {

            m_CurrentHitTime = 0.0f;
            SetPatrolState();

        }
    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Player")
        {
            m_Player.EnemyHit(hit, transform);
        }
    }
    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}