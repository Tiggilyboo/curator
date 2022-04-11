using UnityEngine;

public class LifeformUI: MonoBehaviour
{
    [SerializeField]
    private LifeformInspectorUI m_InspectorUI;

    public void OpenInspector() 
    {
        m_InspectorUI.SetVisible(true);
    }
}
