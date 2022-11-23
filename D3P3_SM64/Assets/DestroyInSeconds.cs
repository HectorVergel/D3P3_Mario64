using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    public float m_Seconds;
    float m_Timer;

    void Update()
    {
        m_Timer += Time.deltaTime;
        
        if(m_Timer >= m_Seconds)
        {
            Destroy(this.gameObject);
        }
    }
}
