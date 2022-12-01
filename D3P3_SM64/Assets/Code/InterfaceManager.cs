using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{

    [Header("GUI")]
    [SerializeField] GameObject m_GUI;
    [SerializeField] Animation m_MyAnimation;
    [SerializeField] AnimationClip m_HideClip;
    [SerializeField] AnimationClip m_ShowClip;
    [SerializeField] int m_CoinAmount;
    [SerializeField] Text m_CoinText;
    [SerializeField] Image m_HealthGUI;
    [SerializeField] float m_HealthAmount;
    [SerializeField] Text m_LifeText;
    [SerializeField] int m_LifeAmount;
    [SerializeField] Image m_DieImage;
    [SerializeField] float m_AlphaSpeed = 1.0f;
    [SerializeField] Image m_RetryImage;
    public float m_TimeToHideGUI;
    float m_Timer = 0.0f;
    bool m_HideGUI;

    
    private void Awake()
    {
        GameController.GetGameController().SetInterface(this);
        m_GUI.SetActive(false);
    }

    private void Update()
    {
        
        if (m_HideGUI)
        {
            HideGUI();
        }
    }

    #region "GUI_MANAGMENT"
    public void ShowGUI()
    {
        m_Timer = 0.0f;
        if (!m_GUI.activeSelf)
        {
            m_GUI.SetActive(true);
            StartCoroutine(ShowState());
        }
    }

    void HideGUI()
    {

        m_Timer += Time.deltaTime;

        if (m_Timer >= m_TimeToHideGUI)
        {
            StartCoroutine(HideState());
            m_Timer = 0.0f;
        }
    }

    IEnumerator HideState()
    {
        m_MyAnimation.Play(m_HideClip.name);
        yield return new WaitForSeconds(m_HideClip.length);
        m_GUI.SetActive(false);
        m_HideGUI = false;
    }


    IEnumerator ShowState()
    {
        m_GUI.SetActive(true);
        m_MyAnimation.Play(m_ShowClip.name);
        yield return new WaitForSeconds(m_ShowClip.length);
        m_HideGUI = true;
    }
    #endregion

    #region "COIN_GUI"

    void UpdateCoinGUI()
    {
        
        m_CoinText.text = m_CoinAmount.ToString();
    }

    public void AddCoin(int _coin)
    {
        m_CoinAmount += _coin;
        UpdateCoinGUI();
        ShowGUI();
    }

    #endregion


    #region "LIFES_GUI"

    public void UpdateHealthGUI(float _Life)
    {
        m_HealthAmount = _Life;
        m_HealthGUI.fillAmount = m_HealthAmount;
    }

    public void UpdateLifeGUI(int _Life)
    {
        m_LifeAmount = _Life;
        m_LifeText.text = m_LifeAmount.ToString();
    }


    #endregion


    public void SetDieInterface(float Time)
    {
        
        StopAllCoroutines();
        StartCoroutine(DieInterfaceFadeIn(Time));
    }

    public void SetRetryInterface()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StopAllCoroutines();
        StartCoroutine(RetryFadeIn());
    }

    IEnumerator RetryFadeIn()
    {
        float l_CurrentAlpha = 0.0f;

        while (m_RetryImage.color.a <= 1.0f)
        {
            l_CurrentAlpha += m_AlphaSpeed * Time.deltaTime;
            m_RetryImage.color = new Color(m_RetryImage.color.r, m_RetryImage.color.g, m_RetryImage.color.b, l_CurrentAlpha);
            yield return null;
        }
       
    }

    public void OnRetryButtonClick()
    {
            
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StopAllCoroutines();
        StartCoroutine(RetryFadeOut());
        GameController.GetGameController().RestartGame();
    }
    IEnumerator RetryFadeOut()
    {
        float l_CurrentAlpha = 1.0f;
        while (m_RetryImage.color.a >= 0f)
        {
            l_CurrentAlpha -= m_AlphaSpeed * Time.deltaTime;
            m_RetryImage.color = new Color(m_RetryImage.color.r, m_RetryImage.color.g, m_RetryImage.color.b, l_CurrentAlpha);
            yield return null;
        }


    }
    IEnumerator DieInterfaceFadeIn(float _Time)
    {
        float l_CurrentAlpha = 0.0f;

        yield return new WaitForSeconds(_Time);
        while (m_DieImage.color.a <= 1.0f)
        {
            l_CurrentAlpha += m_AlphaSpeed * Time.deltaTime;
            m_DieImage.color = new Color(m_DieImage.color.r, m_DieImage.color.g, m_DieImage.color.b, l_CurrentAlpha);
            yield return null;
        }
        GameController.GetGameController().GetPlayer().GetPlayerHealth().RespawnPlayer();
        yield return new WaitForSeconds(2f);
        StartCoroutine(DieInterfaceFadeOut()); 

    }

    IEnumerator DieInterfaceFadeOut()
    {
        float l_CurrentAlpha = 1.0f;
        while (m_DieImage.color.a >= 0f)
        {
            l_CurrentAlpha -= m_AlphaSpeed * Time.deltaTime;
            m_DieImage.color = new Color(m_DieImage.color.r, m_DieImage.color.g, m_DieImage.color.b, l_CurrentAlpha);
            yield return null;
        }


    }
}
