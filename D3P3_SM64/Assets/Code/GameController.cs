using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

    //FPSPlayerController m_Player;
    
    InterfaceManager m_Interface;
    PlayerController m_Player;
    public static GameController m_GameController = null;
    List<IRestartGameElement> m_RestartGameElementList = new List<IRestartGameElement>();
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

    public PlayerController GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
    }


    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElementList.Add(RestartGameElement);
    }

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

        foreach (var GameElement in m_RestartGameElementList)
        {
            GameElement.RestartGame();
        }

    }

   
  

   
}



