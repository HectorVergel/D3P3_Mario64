using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    public LayerMask m_LayerMask;
    Animator m_Animator;
    public AnimationClip m_DeathAnimation;
    public Transform m_UpTransform;
    Rigidbody m_Rigidbody;

    [Header("Health")]
    public int m_MaxHealth = 5;
    int m_CurrentHealth;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    public void TakeDamage(int DamageAmount)
    {
        if(m_CurrentHealth > 0)
        {
            m_CurrentHealth-= DamageAmount;
        }
        else
        {
            Die();
        }
    }

    public void AddForceHit(float Force, Vector3 Direction, Vector3 Position)
    {
        m_Rigidbody.AddForceAtPosition(Direction * Force, Position, ForceMode.Impulse);
    }


    void Update()
    {

    }


    

    public void Die()
    {
        m_CurrentHealth = 0;
        StartCoroutine(SetDie());
        
    }

    IEnumerator SetDie()
    {
        m_Animator.Play(m_DeathAnimation.name);
        yield return new WaitForSeconds(m_DeathAnimation.length);
        gameObject.SetActive(false);
    }
}
