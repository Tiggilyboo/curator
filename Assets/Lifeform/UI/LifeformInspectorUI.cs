using System;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LifeformInspectorUI: MonoBehaviour, IAmUIFor<Lifeform>
{
    [SerializeField]
    private LifeformGeneticsUI m_GeneticsUI;

    [SerializeField]
    private Lifeform m_Lifeform;
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private GraphicRaycaster m_Raycaster;
    [SerializeField]
    private Text m_TitleText;
    [SerializeField]
    private Button m_CloseButton;
    [SerializeField]
    private Text m_BodyText;

    public Lifeform GetComponent() => m_Lifeform;
    public Canvas GetCanvas() => m_Canvas;
    public bool GetVisible() => m_Canvas.isActiveAndEnabled;
    public GraphicRaycaster GetRaycaster() => m_Raycaster;

    public event OnClose OnClose;

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

        sb.AppendLine(m_Lifeform.StateMachine.GetCurrentStateIdentifier());
        sb.Append(keyPair("Age", m_Lifeform.Age, m_Lifeform.Genetics.GetMaxAge()));
        sb.Append(keyPair("Energy", m_Lifeform.Energy, m_Lifeform.Genetics.GetMaxEnergy()));
        sb.Append(keyPair("Hunger", m_Lifeform.Hunger, m_Lifeform.Genetics.GetMaxHunger()));

        return sb.ToString();
    }

    public void SetVisible(bool visible) 
    {
        if(!m_Canvas.isActiveAndEnabled && visible)
        {
            UpdateCanvas();
        }
        m_Canvas.gameObject.SetActive(visible);
        m_GeneticsUI.SetVisible(visible);
    }

    public void OnCloseButtonClick()
    {
        SetVisible(false);
        OnClose?.Invoke();
    }

    void Start()
    {
        SetVisible(false);
    }

    void Update()
    {
        if(GetVisible())
          UpdateCanvas();
    }
}
