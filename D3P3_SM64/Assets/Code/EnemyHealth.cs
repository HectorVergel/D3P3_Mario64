using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMY
{
    GOOMBA,
    KOOPA
}
public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    public LayerMask m_LayerMask;
    Animator m_Animator;
    public AnimationClip m_DeathAnimation;
    public Transform m_UpTransform;
    Rigidbody m_Rigidbody;
    public Transform m_HeadTransform;
    public ENEMY m_CurrentEnemy;



    [Header("Health")]
    public int m_MaxHealth = 5;
    int m_CurrentHealth;
    public float m_ImpactForce = 10f;

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


    

    public void Die()
    {
        if(m_CurrentEnemy == ENEMY.GOOMBA)
        {
            m_CurrentHealth = 0;
            StartCoroutine(SetDie());
        }
        else if (m_CurrentEnemy == ENEMY.KOOPA)
        {
            gameObject.SetActive(false);
            GameObject l_Shell = GetComponent<KoopaSMC>().m_KoopaShell;
            Instantiate(l_Shell, transform.position, Quaternion.identity);
        }
        
        
    }

    IEnumerator SetDie()
    {
        m_Animator.Play(m_DeathAnimation.name);
        yield return new WaitForSeconds(m_DeathAnimation.length);
        gameObject.SetActive(false);
    }
}
