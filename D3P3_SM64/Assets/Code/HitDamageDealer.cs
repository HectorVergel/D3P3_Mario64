using UnityEngine;

public class HitDamageDealer : MonoBehaviour
{
    [Header("Damage")]
    public int m_Damage = 1;
    public float m_ForceHit = 50f;

    [Header("References")]
    public PlayerController m_PlayerController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealth>().TakeDamage(m_Damage);
            other.GetComponent<EnemyHealth>().AddForceHit(m_ForceHit, m_PlayerController.transform.forward, other.transform.position);
        }
    }

}




