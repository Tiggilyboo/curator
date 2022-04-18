using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class LifeformGeneticsUI: MonoBehaviour, IAmUIFor<LifeformGenetics>
{
    [SerializeField]
    private Lifeform m_Lifeform;
    [SerializeField]
    private LifeformGenetics m_Genetics => m_Lifeform.Genetics;
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private GraphicRaycaster m_Raycaster;
    [SerializeField]
    private LifeformTraitsUI m_TraitsUI;
    public LifeformTraitsUI GetTraitsUI() => m_TraitsUI;

    public bool GetVisible() => m_Canvas.isActiveAndEnabled;
    public Canvas GetCanvas() => m_Canvas;
    public LifeformGenetics GetComponent() => m_Genetics;
    public GraphicRaycaster GetRaycaster() => m_Raycaster;

    public void SetVisible(bool visible)
    {
        m_TraitsUI.SetVisible(visible);
    }
}
