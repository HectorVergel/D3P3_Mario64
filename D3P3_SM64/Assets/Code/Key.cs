using UnityEngine;

public class Key : Item
{
    public ParticleSystem m_Particles;
    public override void Pick()
    {
        GameController.GetGameController().GetPlayer().m_HaveKey = true;
        Instantiate(m_Particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
