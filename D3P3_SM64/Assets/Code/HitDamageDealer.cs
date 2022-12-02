using UnityEngine;

public class HitDamageDealer : MonoBehaviour
{
    [Header("Damage")]
    public int m_Damage = 1;
    public float m_ForceHit = 50f;

    [Header("References")]
    public PlayerController m_PlayerController;
    public ParticleSystem m_Particles;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealth>().TakeDamage(m_Damage);
            Instantiate(m_Particles, transform.position, Quaternion.identity);
           
        }
    }

}




