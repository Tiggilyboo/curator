using System;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LifeformInspectorUI: MonoBehaviour
{
    [SerializeField]
    private Lifeform m_Lifeform;
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private Text m_TitleText;
    [SerializeField]
    private Button m_CloseButton;
    [SerializeField]
    private Text m_BodyText;

    public Lifeform Lifeform => m_Lifeform;

    private void UpdateCanvas()
    {
        m_TitleText.text = m_Lifeform.name;
        m_BodyText.text = GetLifeformInformation();
    }

    private string GetLifeformInformation()
    {
        string keyPair(string caption, object num, object denom)
        {
            return string.Format("{0}: {1} / {2}\n", caption, num, denom);
        }

        var sb = new StringBuilder();

        sb.Append(keyPair("Age", m_Lifeform.Age, m_Lifeform.Genetics.GetMaxAge()));
        sb.Append(keyPair("Energy", m_Lifeform.Energy, m_Lifeform.Genetics.GetMaxEnergy()));
        sb.Append(keyPair("Hunger", m_Lifeform.Hunger, m_Lifeform.Genetics.GetMaxHunger()));

        sb.Append("Genetics: ");
        foreach(byte gb in m_Lifeform.Genetics.GetData())
          sb.Append(string.Format("{0} ", gb));
        sb.Append("\n");

        return sb.ToString();
    }

    public void SetVisible(bool visible) 
    {
        if(!m_Canvas.isActiveAndEnabled && visible)
        {
            UpdateCanvas();
        }
        m_Canvas.gameObject.SetActive(visible);
    }

    public void OnCloseButtonClick()
    {
        SetVisible(false);
    }

    void Start()
    {
        SetVisible(false);
    }
}
