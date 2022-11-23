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

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    void Start()
    {

    }


    void Update()
    {

    }


    

    public void Die()
    {
        StartCoroutine(SetDie());
    }

    IEnumerator SetDie()
    {
        m_Animator.Play(m_DeathAnimation.name);
        yield return new WaitForSeconds(m_DeathAnimation.length);
        gameObject.SetActive(false);
    }
}
