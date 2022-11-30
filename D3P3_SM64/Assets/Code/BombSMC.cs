using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombSMC : MonoBehaviour, IRestartGameElement
{
    public enum IState
    {
        PATROL,
        ALERT,
        ATTACK,
        DIE
    }
    [Header("References")]
    [SerializeField] Animator m_Animator;
    [SerializeField] AnimationClip m_ExplosionAnimation;
    [SerializeField] ParticleSystem m_ExplosionParticles;
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
    public float m_TimeToExplode = 3f;
    float m_CurrentTimer;


    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        SetPatrolState();
        GameController.GetGameController().AddRestartGameElement(this);

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




    void SetAttackState()
    {

        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = m_AttackSpeed;

        m_State = IState.ATTACK;
    }
    void UpdateAttackState()
    {
        m_NavMeshAgent.destination = GameController.GetGameController().GetPlayer().transform.position;
        m_CurrentTimer += Time.deltaTime;
        if (m_CurrentTimer >= m_TimeToExplode)
        {

            StartCoroutine(Explosion());
        }

    }
    IEnumerator Explosion()
    {
        m_Animator.Play(m_ExplosionAnimation.name);
        yield return new WaitForSeconds(m_ExplosionAnimation.length);
        Explode();

    }

    void Explode()
    {
        var Particles = Instantiate(m_ExplosionParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
    void SetDieState()
    {

    }
    void UpdateDieState()
    {

    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}