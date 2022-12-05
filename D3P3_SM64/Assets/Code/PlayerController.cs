using UnityEngine;
using UnityEngine.InputSystem;
public enum TPunchType
{
    RIGHT_HAND,
    LEFT_HAND,
    KICK
}
public class PlayerController : MonoBehaviour, IRestartGameElement
{
    [Header("References")]
    Animator m_Animator;
    CharacterController m_CharacterController;
    PlayerHealth m_PlayerHealth;
    public Camera m_Camera;
    public float m_LerpRotation;
    float m_AnimationSpeed;
    public LayerMask m_LayerMask;
    public Vector3 m_StartPosition;
    public Quaternion m_StartRotation;
    public bool m_HaveKey = false;

    [Header("Movement")]
    public float m_WalkSpeed = 2.5f;
    public float m_RunSpeed = 6.5f;
    Vector3 m_Movement;
    public bool m_HasMovement;
    float m_MovementSpeed;
    bool m_IsRunning;

    [Header("Jump")]
    Vector3 m_VerticalSpeed;
    bool m_OnGround = true;
    public float m_JumpSpeed = 10.0f;
    private float m_TimeOnAir;
    public float m_CoyoteTime = 0.0f;
    public float m_FallMultiplier = 2.0f;
    public int m_ExtraJumps = 3;
    public int m_MaxJumps = 3;
    public float m_JumpBoostFactor = 1.25f;
    public float m_TimeToJumpAgain = 0.5f;
    public float m_DownForce = -10.0f;
    public float m_MaxTimeToExtraJump = 5f;
    float m_CurrentJumpTime;
    float m_EndJumpTime;

    [Header("WallJump")]
    public float m_DistanceToWall = 0.2f;
    public LayerMask m_WallLayerMask;
    public float m_GravityOnWallMultiplier = 1f;
    bool m_InWall;
    bool m_JumpFromWall;
    public float m_AngleToDetach = 0.45f;
    float m_TimerCountJump;
    public float m_TimeWallImpulse = 1.0f;
    public float m_WallJumpForce = 10f;
    Vector3 m_WallJumpDirection;
    Vector3 m_InitialForward;



    [Header("Inputs")]
    float m_HorizontalX;
    float m_HorizontalZ;

    [Header("VFX")]
    [SerializeField] ParticleSystem m_LandParticles;
    [SerializeField] ParticleSystem m_StepParticles;
    public Transform m_FeetPosition;

    [Header("Combo")]
    public Collider m_R_Hand;
    public Collider m_L_Hand;
    public Collider m_R_Feet;
    public float m_ComboPunchTime = 2f;
    TPunchType m_CurrentComboPunch;
    float m_ComboPunchCurrentTime;
    bool m_IsPunchActive;

    [Header("Elevator")]
    public float m_ElevatorDotAngle = 0.95f;
    public Collider m_CurrentElevatorCollider = null;

    [Header("Bridge")]
    public float m_BridgeForce = 5.0f;

    [Header("JumpKill")]
    public float m_MaxAngleToKill = 60.0f;
    public float m_SpeedKiller = 10f;

    [Header("Hit")]
    public float m_HitImpact = 10f;
    public float m_MaxTimeHit = 0.5f;
    float m_TimeHit;
    bool m_Hitted;
    Vector3 m_HitDirection;

    [Header("Crouch")]
    bool m_IsCrouching;
    bool m_LongJumping;
    public float m_LongJumpSpeedY = 10f;
    public float m_LongJumpSpeedZ = 5f;
    private void Awake()
    {
        GameController.GetGameController().SetPlayer(this);
        GameController.GetGameController().AddRestartGameElement(this);
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerHealth = GetComponent<PlayerHealth>();

    }
    void Start()
    {
        m_R_Hand.gameObject.SetActive(false);
        m_R_Feet.gameObject.SetActive(false);
        m_L_Hand.gameObject.SetActive(false);
        m_ComboPunchCurrentTime = -m_ComboPunchTime;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;


        GameController.GetGameController().AddRestartGameElement(this);

    }

    public void SetPunchActive(TPunchType PunchType, bool Active)
    {
        if (PunchType == TPunchType.RIGHT_HAND)
        {
            m_R_Hand.gameObject.SetActive(Active);

        }
        else if (PunchType == TPunchType.LEFT_HAND)
        {

            m_L_Hand.gameObject.SetActive(Active);
        }
        else if (PunchType == TPunchType.KICK)
        {

            m_R_Feet.gameObject.SetActive(Active);

        }
    }
    private void OnEnable()
    {
        Inputs.OnMove += SetMoveAxis;
        Inputs.OnJump += SetJump;
        Inputs.OnEndJump += UnsetJump;
        Inputs.OnRun += SetRun;
        Inputs.OnEndRun += UnsetRun;
        Inputs.OnJumpDown += AddForceDown;
        Inputs.OnPunch += SetPunch;
        Inputs.OnCrouch += SetCrouch;
        Inputs.OnEndCrouch += UnsetCrouch;
    }

    private void OnDisable()
    {
        Inputs.OnMove -= SetMoveAxis;
        Inputs.OnJump -= SetJump;
        Inputs.OnEndJump -= UnsetJump;
        Inputs.OnRun -= SetRun;
        Inputs.OnEndRun -= UnsetRun;
        Inputs.OnJumpDown -= AddForceDown;
        Inputs.OnPunch -= SetPunch;
        Inputs.OnCrouch -= SetCrouch;
        Inputs.OnEndCrouch -= UnsetCrouch;

    }

    void LateUpdate()
    {
        if (m_CurrentElevatorCollider != null)
        {
            Vector3 l_EulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0.0f, l_EulerRotation.y, 0.0f);
        }
    }
    void Update()
    {

        Vector3 l_ForwardCamera = m_Camera.transform.forward;
        Vector3 l_RightCamera = m_Camera.transform.right;

        l_ForwardCamera.y = 0.0f;
        l_RightCamera.y = 0.0f;
        m_HasMovement = false;

        m_AnimationSpeed = 0.0f;


        l_ForwardCamera.Normalize();
        l_RightCamera.Normalize();

        m_Movement = Vector3.zero;

        if (m_HorizontalZ > 0.1f)
        {
            m_HasMovement = true;
            m_Movement = l_ForwardCamera;
            m_JumpFromWall = false;
        }

        if (m_HorizontalZ < -0.1f)
        {
            m_HasMovement = true;
            m_Movement = -l_ForwardCamera;
            m_JumpFromWall = false;
        }

        if (m_HorizontalX < -0.1f)
        {
            m_HasMovement = true;
            m_Movement -= l_RightCamera;
            m_JumpFromWall = false;
        }
        if (m_HorizontalX > 0.1f)
        {
            m_HasMovement = true;
            m_Movement += l_RightCamera;
            m_JumpFromWall = false;
        }
        m_MovementSpeed = 0.0f;

        m_Movement.Normalize();


        if (m_HasMovement && !m_Hitted)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(m_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, m_LerpRotation);



            if (m_IsRunning)
            {
                m_AnimationSpeed = 1.0f;
                m_MovementSpeed = m_RunSpeed;
            }
            else
            {
                m_AnimationSpeed = 0.5f;
                m_MovementSpeed = m_WalkSpeed;
            }

            if (m_IsCrouching)
            {
                m_MovementSpeed = 0f;
            }
        }
        else
        {
            m_MovementSpeed = 0f;

        }

        m_Animator.SetFloat("Speed", m_AnimationSpeed);
        m_Animator.SetFloat("VerticalSpeed", m_Movement.y);
        m_Animator.SetBool("Grounded", m_OnGround);
        m_Animator.SetInteger("JumpNumber", m_ExtraJumps);



        if (!m_Hitted && !m_LongJumping && !m_JumpFromWall)
        {
            m_Movement = m_Movement * m_MovementSpeed * Time.deltaTime;
            m_CharacterController.Move(m_Movement);
        }
        else if (m_Hitted)
        {
            HitImpact(m_HitDirection);
        }
        else if (m_LongJumping )
        {
            LongJumpZMovement();
        }
        else if (m_JumpFromWall)
        {
            WallJumpZMovement();
        }
   

        SetGravity();

        CollisionFlags l_collisionFlags = m_CharacterController.Move(m_Movement);

        CheckCollision(l_collisionFlags);


    }

    #region "PUNCH"
    void SetPunch()
    {
        if (CanPunch())
        {
            if (MustRestartComboPunch())
            {
                SetComboPunch(TPunchType.RIGHT_HAND);
            }
            else
            {
                NextComboPunch();
            }
        }
    }

    public void SetIsPunchEnable(bool Active)
    {
        m_IsPunchActive = Active;
    }

    void SetComboPunch(TPunchType PunchType)
    {
        m_CurrentComboPunch = PunchType;
        m_ComboPunchCurrentTime = Time.time;
        m_IsPunchActive = true;

        if (m_CurrentComboPunch == TPunchType.RIGHT_HAND)
        {
            m_Animator.SetTrigger("Punch1");
        }
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
        {
            m_Animator.SetTrigger("Punch2");
        }
        else if (m_CurrentComboPunch == TPunchType.KICK)
        {
            m_Animator.SetTrigger("Punch3");
        }

    }

    void NextComboPunch()
    {
        if (m_CurrentComboPunch == TPunchType.RIGHT_HAND)
        {
            SetComboPunch(TPunchType.LEFT_HAND);
        }
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
        {
            SetComboPunch(TPunchType.KICK);
        }
        else if (m_CurrentComboPunch == TPunchType.KICK)
        {
            SetComboPunch(TPunchType.RIGHT_HAND);

        }
    }

    bool CanPunch()
    {
        return !m_IsPunchActive && m_OnGround;
    }

    bool MustRestartComboPunch()
    {
        return (Time.time - m_ComboPunchCurrentTime) > m_ComboPunchTime;
    }


    #endregion

    public void LockInputs(bool Active)
    {
        if (!Active)
        {
            Inputs.OnMove += SetMoveAxis;
            Inputs.OnJump += SetJump;
        }
        else
        {
            Inputs.OnMove = null;
            Inputs.OnJump = null;
        }
    }


    #region "JUMP"


    void SetGravity()
    {

        m_VerticalSpeed.y += Physics.gravity.y * Time.deltaTime;
        m_Movement.y = m_VerticalSpeed.y * m_GravityOnWallMultiplier * Time.deltaTime;

        if (m_Movement.y <= 0.0f)
        {
            m_Movement.y = m_VerticalSpeed.y * m_FallMultiplier * Time.deltaTime;
        }



    }
    void SetJump()
    {


        if (CanJump())
        {
            m_Animator.SetTrigger("Jump");
            m_CurrentJumpTime = Time.time;
            if (m_ExtraJumps == 0 || Mathf.Abs(m_EndJumpTime - m_CurrentJumpTime) >= m_MaxTimeToExtraJump) m_ExtraJumps = m_MaxJumps;
            m_ExtraJumps -= 1;
            float l_JumpBoost = m_MaxJumps - (m_ExtraJumps * 2f);
            l_JumpBoost = Mathf.Clamp(l_JumpBoost, 1, 1.25f);
            if (!m_IsCrouching)
                m_VerticalSpeed.y = m_JumpSpeed * l_JumpBoost;
            else
                LongJump();


        }

        if (m_InWall && !m_OnGround)
        {
            LockInputs(true);
            m_JumpFromWall = true;
            m_WallJumpDirection = -m_InitialForward;
            transform.forward = m_WallJumpDirection;
            m_VerticalSpeed.y = m_JumpSpeed;


        }

    }
    void LongJumpZMovement()
    {
        m_Movement = Vector3.zero;
        m_CharacterController.Move(m_LongJumpSpeedZ * transform.forward * Time.deltaTime);
    }

    void WallJumpZMovement()
    {
        m_Movement = Vector3.zero;
        m_CharacterController.Move(m_WallJumpForce * m_WallJumpDirection * Time.deltaTime);

        m_TimerCountJump += Time.deltaTime;

        if (m_TimerCountJump >= m_TimeWallImpulse)
        {
            LockInputs(false);
        }
    }
    void LongJump()
    {
        m_LongJumping = true;
        m_VerticalSpeed.y = m_LongJumpSpeedY;

    }


    bool CanJump()
    {
        return m_OnGround;
    }


    void UnsetJump()
    {
    }

    #endregion



    void SetCrouch()
    {
        m_IsCrouching = true;

        if (!m_OnGround)
        {
            m_VerticalSpeed.y = m_DownForce;
            m_Animator.SetTrigger("Bum");

        }
        else
        {

            m_Animator.SetBool("Crouching", m_IsCrouching);
        }
    }

    void UnsetCrouch()
    {
        m_IsCrouching = false;
        m_Animator.SetBool("Crouching", m_IsCrouching);
    }

    void SetMoveAxis(float x, float z)
    {
        m_HorizontalX = x;
        m_HorizontalZ = z;
    }

    void SetRun()
    {
        m_IsRunning = true;
    }

    void UnsetRun()
    {
        m_IsRunning = false;
    }




    public void AddForceUp(float _Force)
    {
        m_VerticalSpeed.y = _Force;
    }

    public void AddForceHorizontal(float Force, Vector3 Direction)
    {
        m_Movement.z += Direction.z * Force;
    }

    void AddForceDown()
    {
        if (!m_OnGround)
        {
            m_VerticalSpeed.y = m_DownForce;
            m_Animator.SetTrigger("Bum");

        }
    }


    void CheckCollision(CollisionFlags collisionFlag)
    {
        if ((collisionFlag & CollisionFlags.Above) != 0 && m_VerticalSpeed.y > 0.0f)
        {
            m_VerticalSpeed.y = 0.0f;
        }

        if ((collisionFlag & CollisionFlags.Below) != 0 && m_VerticalSpeed.y < 0.0f)
        {

            m_WallJumpDirection = Vector3.zero;
            m_LongJumping = false;
            m_JumpFromWall = false;
            m_VerticalSpeed.y = 0.0f;
            m_TimeOnAir = 0.0f;
            m_OnGround = true;

        }
        else
        {
            m_TimeOnAir += Time.deltaTime;
            if (m_TimeOnAir > m_CoyoteTime)
                m_OnGround = false;
        }
    }

    public void LandParticles()
    {

        m_EndJumpTime = Time.time;
        ParticleSystem vfx = Instantiate(m_LandParticles);
        vfx.gameObject.transform.position = m_FeetPosition.position;
    }

    public void StepParticles()
    {
        ParticleSystem vfx = Instantiate(m_StepParticles);
        vfx.gameObject.transform.position = m_FeetPosition.position;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Elevator" && CanAttachToElevator(other))
        {
            AttachToElevator(other);

        }
        if (other.tag == "Wall" && CanAttachToWall())
        {
            AttachToWall();
            m_InitialForward = transform.forward;
        }

    }



    void AttachToWall()
    {
        m_FallMultiplier = 0.25f;
        m_ExtraJumps = 2;
        m_InWall = true;
    }

    void DetachWall()
    {
        m_FallMultiplier = 2.0f;
        m_InWall = false;
    }
    bool CanAttachToWall()
    {
        return true;
    }

    bool MustDetachWall(Vector3 OtherForward)
    {

        return Vector3.Dot(transform.forward, -OtherForward) <= m_AngleToDetach;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Elevator" && CanAttachToElevator(other) && Vector3.Dot(other.transform.up, transform.up) < m_ElevatorDotAngle)
        {
            AttachToElevator(other);
        }
        if (other.tag == "Wall" && MustDetachWall(other.transform.forward))
        {
            DetachWall();
        }
        if (other.tag == "Wall" && !MustDetachWall(other.transform.forward))
        {
            AttachToWall();
        }

    }
    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Elevator")
        {
            if (other == m_CurrentElevatorCollider)
                DetachElevator();

        }
        if (other.tag == "Wall")
        {
            DetachWall();
        }
    }

    void AttachToElevator(Collider other)
    {

        transform.SetParent(other.transform);
        m_CurrentElevatorCollider = other;
    }

    void DetachElevator()
    {
        transform.SetParent(null);
        m_CurrentElevatorCollider = null;
    }
    bool CanAttachToElevator(Collider other)
    {
        return m_CurrentElevatorCollider == null && Vector3.Dot(other.transform.up, transform.up) >= m_ElevatorDotAngle;
    }


    public CharacterController GetCharacterController()
    {
        return m_CharacterController;
    }

    public PlayerHealth GetPlayerHealth()
    {
        return m_PlayerHealth;
    }


    public void StopMovement()
    {
        m_CharacterController.Move(Vector3.zero);
    }

    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        LockInputs(false);
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Bridge")
        {
            hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
        }
        else if (hit.gameObject.tag == "Enemy")
        {

            EnemyHit(hit, transform);
        }
        else if (hit.gameObject.tag == "Shell")
        {
            hit.rigidbody.AddForce(transform.forward * 80f, ForceMode.Impulse);
        }
    }

    public void EnemyHit(ControllerColliderHit hit, Transform Entity)
    {
        if (CanKill(hit.normal))
        {
            hit.gameObject.GetComponent<EnemyHealth>().Die();
            AddForceUp(m_SpeedKiller);
        }
        else
        {
            PlayerTakeDamage(Entity);
        }
    }

    public void PlayerTakeDamage(Transform Entity)
    {
        m_PlayerHealth.TakeDamage(0);
        Vector3 l_Direction = Entity.position - transform.position;
        l_Direction.Normalize();
        l_Direction.y = 0f;

        FindObjectOfType<GoombaSMC>().SetHitState(l_Direction);
        m_HitDirection = -l_Direction;
        m_Hitted = true;
    }

    void HitImpact(Vector3 Direction)
    {
        m_Movement = Vector3.zero;
        m_Animator.SetTrigger("Hit");
        LockInputs(true);

        m_CharacterController.Move(Direction * m_HitImpact * Time.deltaTime);

        m_TimeHit += Time.deltaTime;

        if (m_TimeHit >= m_MaxTimeHit)
        {

            m_Hitted = false;
            LockInputs(false);
            m_TimeHit = 0f;
        }
    }

    bool CanKill(Vector3 normal)
    {
        return Vector3.Dot(normal, Vector3.up) >= Mathf.Cos(m_MaxAngleToKill * Mathf.Deg2Rad);
    }

}




