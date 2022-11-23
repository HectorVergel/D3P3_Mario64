public class Coin : Item
{
    public int m_CoinValue = 1;
    public override void Pick()
    {
        GameController.GetGameController().GetInterface().AddCoin(m_CoinValue);
        GameController.GetGameController().GetInterface().ShowGUI();
        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}



