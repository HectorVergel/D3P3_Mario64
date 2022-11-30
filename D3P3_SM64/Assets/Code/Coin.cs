using UnityEngine;

public class Coin : Item, IRestartGameElement
{
    public int m_CoinValue = 1;
    public ParticleSystem m_Particles;

    private void Start()
    {
        GameController.GetGameController().AddRestartGameElement(this);
    }
    public override void Pick()
    {
        GameController.GetGameController().GetInterface().AddCoin(m_CoinValue);
        Instantiate(m_Particles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}



