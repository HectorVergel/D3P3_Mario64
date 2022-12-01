using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform m_LookAtTransform;

    public float m_MaxDistance = 6.0f;
    public float m_MinDistance = 2.0f;

    public float m_YawRotationSpeed = 720.0f;
    public float m_PitchRotationSpeed = 360.0f;

    float m_Pitch;
    public float m_MaxPitch = 20.0f;
    public float m_MinPitch = -60.0f;


    [Header("AvoidObject")]
    public LayerMask m_LayerMask;
    public float m_AvoidOffset = 0.1f;



    [Header("Debug")]
    public KeyCode m_DebugLockedAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockedKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_MouseLocked = false;

    private void Start()
    {
       
    }


#if UNITY_EDITOR
    void UpdateInputDebug()
    {
        if (Input.GetKeyDown(m_DebugLockedAngleKeyCode))
        {
            m_AngleLocked = !m_AngleLocked;

            
        }

        if (Input.GetKeyDown(m_DebugLockedKeyCode))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Locked;
                m_MouseLocked = Cursor.lockState == CursorLockMode.Locked;
            }

        }
    }
#endif
    private void LateUpdate()
    {
#if UNITY_EDITOR
        UpdateInputDebug();
#endif

        float l_MouseX;
        float l_MouseY;
        if (IsMouseActive())
        {
            l_MouseX = Input.GetAxis("Mouse X");
            l_MouseY = Input.GetAxis("Mouse Y");
        }
        else
        {
            l_MouseX = Input.GetAxis("Horizontal");
            l_MouseY = Input.GetAxis("Vertical");
        }
        

#if UNITY_EDITOR
        if (m_AngleLocked)
        {
            l_MouseX = 0.0f;
            l_MouseY = 0.0f;
        }
#endif

        transform.LookAt(m_LookAtTransform.position);
        float l_Distance = Vector3.Distance(transform.position, m_LookAtTransform.position);
        Vector3 l_EulerAngles = transform.rotation.eulerAngles;
        float l_Yaw = l_EulerAngles.y;

        l_Yaw += l_MouseX * m_YawRotationSpeed * Time.deltaTime;
        
        m_Pitch += l_MouseY * m_PitchRotationSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);


        Vector3 l_ForwardCamera = new Vector3(Mathf.Sin(l_Yaw * Mathf.Deg2Rad) * Mathf.Cos(m_Pitch * Mathf.Deg2Rad), 
            Mathf.Sin(m_Pitch * Mathf.Deg2Rad), Mathf.Cos(l_Yaw * Mathf.Deg2Rad) * Mathf.Cos(m_Pitch * Mathf.Deg2Rad));
        l_Distance = Mathf.Clamp(l_Distance, m_MinDistance, m_MaxDistance);
        Vector3 l_DesiredPosition = m_LookAtTransform.position - l_ForwardCamera * l_Distance;

        Ray l_Ray = new Ray(m_LookAtTransform.position, -l_ForwardCamera);
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance, m_LayerMask.value))
        {
            l_DesiredPosition = l_RaycastHit.point + l_ForwardCamera * m_AvoidOffset;
        }

        transform.position = l_DesiredPosition;
        transform.LookAt(m_LookAtTransform.position);
    }


    bool IsMouseActive()
    {
        return Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f;
    }
}
