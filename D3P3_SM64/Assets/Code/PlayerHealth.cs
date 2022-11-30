using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float m_MaxHealth = 1.0f;
    float m_CurrentHealth;
    public int m_PlayerLifes = 3;
    public float m_SliceValue = 1/8f;

    [Header("References")]
    InterfaceManager m_Interface;
    PlayerController m_Controller;

    [Header("Checkpoints")]
    [SerializeField] Transform m_CurrentCheckpoint;


   
    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_Interface = GameController.GetGameController().GetInterface();
        m_Controller = GameController.GetGameController().GetPlayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
    }
    public void TakeDamage(int _Amount)
    {
        float l_DealDamage = _Amount * m_SliceValue;
        m_Interface.ShowGUI();
        if (m_CurrentHealth > 0.0f)
        {
            m_CurrentHealth -= l_DealDamage;
            m_Interface.UpdateHealthGUI(m_CurrentHealth);

        }
        else if(m_CurrentHealth <= 0.0f)
        {
            OnDie();
        }
    }

    
    public void RespawnPlayer()
    {
        m_Controller.GetCharacterController().enabled = false;
        transform.position = m_CurrentCheckpoint.position;
        m_Controller.GetCharacterController().enabled = true;
    }
    public void OnDie()
    {
        if(m_PlayerLifes > 0)
        {
            m_PlayerLifes -= 1;
            m_CurrentHealth = m_MaxHealth;
            m_Interface.SetDieInterface();
            m_Interface.UpdateHealthGUI(m_CurrentHealth);
            

        }
        else
        {
            GameController.GetGameController().RestartGame();
        }
    }

    public void RestartGame()
    {
        RespawnPlayer();
        m_PlayerLifes = 3;
    }
}