using System.Collections.Generic;
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
    public Camera m_Camera;
    public float m_LerpRotation;
    float m_AnimationSpeed;
    public LayerMask m_LayerMask;
    public Vector3 m_StartPosition;
    public Quaternion m_StartRotation;

    [Header("Movement")]
    public float m_WalkSpeed = 2.5f;
    public float m_RunSpeed = 6.5f;
    Vector3 m_Movement;
    bool m_HasMovement;
    float m_MovementSpeed;
    bool m_IsRunning;

    [Header("Jump")]
    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = true;
    public float m_JumpSpeed = 10.0f;
    private float m_TimeOnAir;
    public float m_CoyoteTime = 0.0f;
    public float m_FallMultiplier = 2.0f;
    public int m_ExtraJumps = 2;
    public float m_TimeToJumpAgain = 0.5f;
    public float m_CurrentTimeJump;
    bool m_JumpPressed;
    bool m_OnTime;
    public float m_DownForce = -10.0f;

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
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();

    }
    void Start()
    {
        m_R_Hand.gameObject.SetActive(false);
        m_R_Feet.gameObject.SetActive(false);
        m_L_Hand.gameObject.SetActive(false);
        m_ComboPunchCurrentTime = -m_ComboPunchTime;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        GameController.GetGameController().SetPlayer(this);
        GameController.GetGameController().AddRestartGameElement(this);
    }

    public void SetPunchActive(TPunchType PunchType, bool Active)
    {
        if(PunchType == TPunchType.RIGHT_HAND)
        {
            m_R_Hand.gameObject.SetActive(Active);
          
        }
        else if(PunchType == TPunchType.LEFT_HAND)
        {
           
            m_L_Hand.gameObject.SetActive(Active);
        }
        else if(PunchType == TPunchType.KICK)
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
    }

    private void OnDisable()
    {
        Inputs.OnMove -= SetMoveAxis;
        Inputs.OnJump -= SetJump;
        Inputs.OnEndJump -= UnsetJump;
        Inputs.OnRun -= SetRun;
        Inputs.OnEndRun -= UnsetRun;
        Inputs.OnJumpDown -= AddForceDown;

    }

    void LateUpdate()
    {
        if(m_CurrentElevatorCollider == null)
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
        }
        if (m_HorizontalZ < -0.1f)
        {
            m_HasMovement = true;
            m_Movement = -l_ForwardCamera;
        }
        if (m_HorizontalX < -0.1f)
        {
            m_HasMovement = true;
            m_Movement -= l_RightCamera;
        }
        if (m_HorizontalX > 0.1f)
        {
            m_HasMovement = true;
            m_Movement += l_RightCamera;
        }
        m_MovementSpeed = 0.0f;

        m_Movement.Normalize();

        if (m_JumpPressed && m_Movement.y < 0.0f)
        {
            SetTimer();
        }

        if (m_HasMovement)
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
        }

        m_Animator.SetFloat("Speed", m_AnimationSpeed);
        m_Animator.SetFloat("VerticalSpeed", m_Movement.y);
        m_Animator.SetBool("Grounded", m_OnGround);
        m_Animator.SetInteger("JumpNumber", m_ExtraJumps);
        m_Movement = m_Movement * m_MovementSpeed * Time.deltaTime;




        if (Input.GetMouseButtonDown(0) && CanPunch())
        {
            if (MustRestartComboPunch())
            {
                SetComboPunch(TPunchType.RIGHT_HAND);
            }
            else
            {
                Debug.Log("IN");
                NextComboPunch();
            }
        }

        m_CharacterController.Move(m_Movement);


        SetGravity();


        CollisionFlags l_collisionFlags = m_CharacterController.Move(m_Movement);

        CheckCollision(l_collisionFlags);

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

        if (m_CurrentComboPunch ==TPunchType.RIGHT_HAND)
        {
            m_Animator.SetTrigger("Punch1");
        }
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
        {
            m_Animator.SetTrigger("Punch2");
        }
        else if(m_CurrentComboPunch == TPunchType.KICK)
        {
            m_Animator.SetTrigger("Punch3");
        }

    }

    void NextComboPunch()
    {
        if(m_CurrentComboPunch == TPunchType.RIGHT_HAND)
        {
            SetComboPunch(TPunchType.LEFT_HAND);
        }
        else if(m_CurrentComboPunch == TPunchType.LEFT_HAND)
        {
            SetComboPunch(TPunchType.KICK);
        }
        else if(m_CurrentComboPunch == TPunchType.KICK)
        {
            SetComboPunch(TPunchType.RIGHT_HAND);

        }
    }
    void SetJump()
    {

        m_JumpPressed = true;


        if (CanJump())
        {
            m_Animator.SetTrigger("Jump");
            m_VerticalSpeed = m_JumpSpeed;
        }
        else
        {
            if (CanExtraJump())
            {
                m_Animator.SetTrigger("Jump");
                m_ExtraJumps -= 1;
                m_VerticalSpeed = m_JumpSpeed;
            }
        }
    }

    void SetTimer()
    {
        m_CurrentTimeJump += Time.deltaTime;

        if (m_CurrentTimeJump >= m_TimeToJumpAgain)
        {
            m_OnTime = false;
        }
        else
        {
            m_OnTime = true;
        }
    }

    bool CanJump()
    {
        return m_OnGround;
    }

    bool CanExtraJump()
    {
        return !m_OnGround && IsFalling() && m_ExtraJumps > 0;
    }

    void UnsetJump()
    {
        m_JumpPressed = false;
    }

    bool IsFalling()
    {

        return m_Movement.y <= 0.0f && !m_OnGround;
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



    /* void UpdateRun()
     {
         if (m_HasMovement && m_IsRunning)
         {
             Debug.Log("Speed");
             m_AnimationSpeed = 1.0f;
             m_MovementSpeed = m_RunSpeed;

         }
     }
 */

    void SetGravity()
    {
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        m_Movement.y = m_VerticalSpeed * Time.deltaTime;

        if (m_Movement.y <= 0.0f)
        {
            m_Movement.y = m_VerticalSpeed * m_FallMultiplier * Time.deltaTime;
        }

    }

    public void AddForceUp(float _Force)
    {
        m_VerticalSpeed = _Force;
    }

    void AddForceDown()
    {
        if (!m_OnGround)
        {
            m_VerticalSpeed = m_DownForce;
            m_Animator.SetTrigger("Bum");

        }
    }

    void CheckIfWallOnFront()
    {

    }

    void CheckCollision(CollisionFlags collisionFlag)
    {
        if ((collisionFlag & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }

        if ((collisionFlag & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_TimeOnAir = 0.0f;
            m_OnGround = true;
            m_ExtraJumps = 2;




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
        if (other.tag == "Enemy")
        {
            if (CheckIfOnTop())
            {
                other.GetComponent<EnemyHealth>().Die();
                AddForceUp(10f);
            }
        }
        if(other.tag == "Elevator" && CanAttachToElevator(other))
        {
            AttachToElevator(other);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Elevator" && CanAttachToElevator(other) && Vector3.Dot(other.transform.up,transform.up) < m_ElevatorDotAngle)
        {
            AttachToElevator(other);
        }
        if (CanAttachToElevator(other))
        {
            AttachToElevator(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
     
        if (other.tag == "Elevator")
        {
            if(other == m_CurrentElevatorCollider)
                DetachElevator();

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

    bool CheckIfOnTop()
    {
        RaycastHit l_RayHit;
        Ray l_Ray = new Ray(m_FeetPosition.position, Vector3.down);

        if (Physics.Raycast(l_Ray, out l_RayHit, m_LayerMask.value))
        {
            return true;
        }
        return false;
    }

    bool CanPunch()
    {
        return !m_IsPunchActive;
    }

    bool MustRestartComboPunch()
    {
        return (Time.time - m_ComboPunchCurrentTime) > m_ComboPunchTime;
    }

    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Bridge")
        {
            hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
        }   
    }

}


public class TimeScalerDebug : MonoBehaviour
{

    public KeyCode m_FastKeyCode = KeyCode.RightControl;
    public KeyCode m_SlowKeyCode = KeyCode.LeftAlt;
#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKeyDown(m_FastKeyCode))
        {
            Time.timeScale = 10.0f;
        }
        if (Input.GetKeyDown(m_SlowKeyCode))
        {
            Time.timeScale = 0.5f;
        }

        if (Input.GetKeyUp(m_FastKeyCode) || Input.GetKeyUp(m_SlowKeyCode))
        {
            Time.timeScale = 1;
        }
    }

}
#endif