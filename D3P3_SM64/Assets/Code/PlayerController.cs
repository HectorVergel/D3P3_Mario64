using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    Animator m_Animator;
    CharacterController m_CharacterController;
    public Camera m_Camera;
    public float m_LerpRotation;
    float m_AnimationSpeed;

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
    float m_Gravity = -9.8f;
    public float m_MaxJumpHeight = 1.0f;
    public float m_MaxJumpTime = 0.5f;
    float m_InitialJumpVelocity;
    bool m_JumpPressed;
    public float m_GravityOnGround = -0.05f;

    [Header("Inputs")]
    float m_HorizontalX;
    float m_HorizontalZ;


    private void Awake()
    {

        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();

        SetJumpVariables();


    }
    void Start()
    {

    }

    private void OnEnable()
    {
        Inputs.OnMove += SetMoveAxis;
        Inputs.OnJump += SetJump;
        Inputs.OnEndJump += UnsetJump;
        Inputs.OnRun += SetRun;
        Inputs.OnEndRun += UnsetRun;
    }

    private void OnDisable()
    {
        Inputs.OnMove -= SetMoveAxis;
        Inputs.OnJump -= SetJump;
        Inputs.OnEndJump -= UnsetJump;
        Inputs.OnRun -= SetRun;
        Inputs.OnEndRun -= UnsetRun;
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
        if (m_HasMovement)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(m_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, m_LerpRotation);

            m_AnimationSpeed = 0.5f;
            m_MovementSpeed = m_WalkSpeed;

            if (m_IsRunning)
            {
                m_AnimationSpeed = 1.0f;
                m_MovementSpeed = m_RunSpeed;
            }
        }

        m_Animator.SetFloat("Speed", m_AnimationSpeed);
        m_Movement = m_Movement * m_MovementSpeed * Time.deltaTime;




        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_Animator.SetTrigger("Punch");
        }

        m_CharacterController.Move(m_Movement);

        //SetGravity();
        UpdateGravity();
        DoJump();

        CollisionFlags l_collisionFlags = m_CharacterController.Move(m_Movement);

        CheckCollision(l_collisionFlags);
    }

    //New Jump
    void SetJumpVariables()
    {
        float l_TimeToApex = m_MaxJumpTime / 2.0f;
        m_Gravity = (-2 * m_MaxJumpHeight) / Mathf.Pow(l_TimeToApex, 2);
        m_InitialJumpVelocity = (2 * m_MaxJumpHeight) / l_TimeToApex;
        
    }

    void DoJump()
    {

        if (m_OnGround && m_JumpPressed)
        {
            float l_PreviousYVelocity = m_Movement.y;
            float l_NewYVelocity = m_Movement.y + m_InitialJumpVelocity;
            float l_NextVelocity = (l_PreviousYVelocity + l_NewYVelocity) * 0.5f;
            m_Movement.y = l_NextVelocity;
            Debug.Log(m_Movement.y);

        }
    }


    void SetJump()
    {
        m_JumpPressed = true;
    }

    void UnsetJump()
    {
        m_JumpPressed = false;
    }


    void UpdateGravity()
    {
        if (m_OnGround)
        {
            m_Movement.y = m_GravityOnGround;
        }
        else
        {
            float l_PreviousYVelocity = m_Movement.y;
            float l_NewYVelocity = m_Movement.y + (m_Gravity * Time.deltaTime);
            float l_NextVelocity = (l_PreviousYVelocity + l_NewYVelocity) * 0.5f;
            m_Movement.y += l_NextVelocity;
        }
    }
    //End New Jump
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

    void UpdateMovement()
    {

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
    }

    void UpdateJump()
    {
        if (m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }

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
        }
        else
        {
            m_TimeOnAir += Time.deltaTime;
            if (m_TimeOnAir > m_CoyoteTime)
                m_OnGround = false;
        }
    }
}
