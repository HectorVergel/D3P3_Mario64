using UnityEngine;

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
#endif
}
