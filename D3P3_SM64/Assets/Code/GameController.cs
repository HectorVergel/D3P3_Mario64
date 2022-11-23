using UnityEngine;


public class GameController : MonoBehaviour
{

    //FPSPlayerController m_Player;
    
    InterfaceManager m_Interface;
    public static GameController m_GameController = null;
    private void Start()
    {
        //SetPortals();
        //SetAllEnemies();
        DontDestroyOnLoad(this.gameObject);
        Cursor.visible = false;

    }

    public static GameController GetGameController()
    {
        if (m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
        }
        return m_GameController;
    }

    public static void DestroySingleton()
    {
        if (m_GameController != null)
        {
            GameObject.Destroy(m_GameController.gameObject);
        }
        m_GameController = null;
    }

    /*public FPSPlayerController GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayer(FPSPlayerController _player)
    {
        m_Player = _player;
    }*/



    public void SetInterface(InterfaceManager _interfacePlayer)
    {
        m_Interface = _interfacePlayer;
    }

    public InterfaceManager GetInterface()
    {
        return m_Interface;
    }
    public void RestartGame()
    {
        
        //m_Player.RestartGame();
        //RestartEnemies();

    }

    public void SetAllEnemies()
    {
        //m_Enemies = FindObjectsOfType<Turret>();
    }

  

    /*void RestartEnemies()
    {
        foreach (Turret enemy in m_Enemies)
        {
            enemy.RestartGame();
        }
    }*/
}



