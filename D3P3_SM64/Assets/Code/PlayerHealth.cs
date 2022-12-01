using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float m_MaxHealth = 1.0f;
    float m_CurrentHealth;
    public int m_MaxPlayerLifes = 3;
    int m_CurrentLifes;
    public float m_SliceValue = 1 / 8f;

    [Header("References")]
    InterfaceManager m_Interface;
    PlayerController m_Controller;
    Animator m_Animator;
    public AnimationClip m_DeathAnimation;

    [Header("Checkpoints")]
    [SerializeField] Transform m_CurrentCheckpoint;



    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_CurrentLifes = m_MaxPlayerLifes;
        m_Interface = GameController.GetGameController().GetInterface();
        m_Controller = GameController.GetGameController().GetPlayer();
        m_Animator = GetComponent<Animator>();

        m_Interface.UpdateHealthGUI(m_MaxHealth);
        m_Interface.UpdateLifeGUI(m_CurrentLifes);
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
        m_CurrentHealth -= l_DealDamage;
        m_Interface.UpdateHealthGUI(m_CurrentHealth);

        if (m_CurrentHealth <= 0.0f)
        {
            OnDie();
        }
    }


    public void RespawnPlayer()
    {
        GameController.GetGameController().GetPlayer().GetCharacterController().enabled = false;
        GameController.GetGameController().GetPlayer().LockInputs(false);
        transform.position = m_CurrentCheckpoint.position;
        m_CurrentHealth = m_MaxHealth;
        m_Interface.ShowGUI();
        GameController.GetGameController().GetPlayer().GetCharacterController().enabled = true;
    }
    public void OnDie()
    {
        m_Animator.SetTrigger("Death");
        GameController.GetGameController().GetPlayer().LockInputs(true);
        GameController.GetGameController().GetPlayer().StopMovement();
        if (m_CurrentLifes > 0)
        {
            m_CurrentLifes -= 1;
            m_Interface.SetDieInterface(m_DeathAnimation.length);
            m_Interface.UpdateHealthGUI(m_CurrentHealth);
            m_Interface.UpdateLifeGUI(m_CurrentLifes);


        }
        else
        {
            GameController.GetGameController().GetInterface().SetRetryInterface();
        }
    }

    public void RestartGame()
    {
        m_CurrentLifes = m_MaxPlayerLifes;
        m_CurrentHealth = m_MaxHealth;
        m_Interface.UpdateHealthGUI(m_CurrentHealth);
        m_Interface.UpdateLifeGUI(m_CurrentLifes);
        m_Interface.ShowGUI();
    }
}