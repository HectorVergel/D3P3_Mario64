using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Animation m_Animation;
    [SerializeField] AnimationClip m_StarAnimationClip;

    bool m_IsOpen = false;
    void SpawnStar()
    {
        if (!m_IsOpen)
        {
            m_Animation.Play(m_StarAnimationClip.name);
            m_IsOpen = true;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().m_HaveKey)
            {
                SpawnStar();
            }
            
        }
    }
}
