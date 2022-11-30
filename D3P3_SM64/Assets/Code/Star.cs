using UnityEngine;

public class Star : Item
{
    public int m_CoinValue = 50;
    public ParticleSystem m_Particles; 
    public override void Pick()
    {
        GameController.GetGameController().GetInterface().AddCoin(m_CoinValue);
        Instantiate(m_Particles, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
