using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverUI: MonoBehaviour, IAmUI, IPointerEnterHandler, IPointerExitHandler
{
    private float m_OpenedAt;

    [SerializeField]
    private List<Text> m_TextElements;

    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private GraphicRaycaster m_Raycaster;
    [SerializeField]
    private GameObject m_Panel;
    [SerializeField]
    private VerticalLayoutGroup m_Layout;
    [SerializeField]
    private bool m_KeepAlive;
    [SerializeField]
    private float m_HoverCloseDelayInSec = 2f;

    public event OnClose OnClose;
    public event OnPointer OnMouseEnter;
    public event OnPointer OnMouseExit;
    public Canvas GetCanvas() => m_Canvas;
    public GraphicRaycaster GetRaycaster() => m_Raycaster;
    public bool GetVisible() => m_Canvas.isActiveAndEnabled;
    public void SetPosition(Vector3 pos) => transform.position = pos;
    public void Close() => SetVisible(false);
    
    public void SetVisible(bool visible) 
    {
        if(GetVisible() && !visible)
            OnClose.Invoke();

        m_Canvas.gameObject.SetActive(visible);
        m_OpenedAt = Time.realtimeSinceStartup;
        m_KeepAlive = true;
    }

    public void Initialise(IEnumerable<Text> textElements)
    {
        m_TextElements = textElements.ToList();

        foreach(Text e in textElements)
            e.gameObject.transform.SetParent(m_Layout.transform);

        SetVisible(true);
    }

    public void UpdateTextElement(int textElementIdx, string text)
    {
        Text element = m_TextElements.ElementAt(textElementIdx);
        element.text = text;
    }

    public void OnPointerEnter(PointerEventData pointer)
    {
        // Keep the UI open
        SetVisible(true);

        OnMouseEnter?.Invoke(pointer);
    }
    
    public void OnPointerExit(PointerEventData pointer)
    {
        OnMouseExit?.Invoke(pointer);
    }

    void Start()
    {
        if(m_TextElements == null)
          m_TextElements = new List<Text>();
    }

    void Update()
    {
        if(!m_KeepAlive)
        {
            if(Time.realtimeSinceStartup < m_OpenedAt + m_HoverCloseDelayInSec)
              return;

            SetVisible(false);
        }
    }
}
