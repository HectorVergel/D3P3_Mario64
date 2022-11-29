using UnityEngine;

public class DamageDealer : MonoBehaviour
{

    public int m_Damage = 1;
    public float m_Force = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(m_Damage);
            other.GetComponent<PlayerController>().AddForceHorizontal(m_Force, -other.transform.forward);
        }
    }


}




