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
    [SerializeField] Image m_LifeGUI;
    [SerializeField] float m_LifeAmount;
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
    }

    #endregion


    #region "LIFES_GUI"

    public void UpdateLifeGUI(float _Life)
    {
        m_LifeAmount = _Life;
        m_LifeGUI.fillAmount = m_LifeAmount;
    }


    #endregion
}
