using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float m_MaxHealth = 1.0f;
    float m_CurrentHealth;
    public int m_PlayerLifes = 3;
    public float m_SliceValue = 0.123f;

    [Header("References")]
    InterfaceManager m_Interface;
    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_Interface = GameController.GetGameController().GetInterface();
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
            m_Interface.UpdateLifeGUI(m_CurrentHealth);

        }
        else
        {
            OnDie();
        }
    }

    public void OnDie()
    {
        if(m_PlayerLifes > 0)
        {
            m_PlayerLifes -= 1;
        }
        else
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {

    }
}