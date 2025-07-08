using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Text m_loadingCountingTxt;
    [SerializeField] private Image m_loadingFilled;

    public void UpdateUI(float loadingProgress)
    {
        if(m_loadingCountingTxt)
        {
            m_loadingCountingTxt.text = $"Loading...{(loadingProgress * 100).ToString("f0")}%";
        }
        if(m_loadingFilled)
        {
            m_loadingFilled.fillAmount = loadingProgress;
        }
    }
}
