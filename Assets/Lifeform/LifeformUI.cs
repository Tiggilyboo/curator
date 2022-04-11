using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeformUI: MonoBehaviour
{
    // TODO: This is probably too dodgy, could also make this a part of curator / the player?
    private static List<LifeformInspectorUI> m_ActiveInspectors = new List<LifeformInspectorUI>();

    [SerializeField]
    private LifeformInspectorUI m_InspectorUI;

    void OnMouseDown()
    {
        Debug.Log("LifeformUI.OnMouseUp triggered!");
        for(int i = 0; i < m_ActiveInspectors.Count; i++)
        {
            LifeformInspectorUI otherUI = m_ActiveInspectors[i];
            otherUI.SetVisible(false);
            m_ActiveInspectors.RemoveAt(i);
        }
        m_InspectorUI.SetVisible(true);
        m_ActiveInspectors.Add(m_InspectorUI);
    }
}
