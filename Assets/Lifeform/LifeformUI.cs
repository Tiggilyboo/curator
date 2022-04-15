using UnityEngine;

public class LifeformUI: MonoBehaviour
{
    [SerializeField]
    private LifeformInspectorUI m_InspectorUI;

    public LifeformInspectorUI OpenInspector() 
    {
        m_InspectorUI.SetVisible(true);

        return m_InspectorUI;
    }
}
