using UnityEngine;
using UnityEngine.UI;

public class LifeformUI: MonoBehaviour
{
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private GraphicRaycaster m_GraphicRaycaster;

    [SerializeField]
    private LifeformInspectorUI m_InspectorUI;

    public Canvas GetCanvas() => m_Canvas;
    public GraphicRaycaster GetRaycaster() => m_GraphicRaycaster;

    public LifeformInspectorUI OpenInspector() 
    {
        m_InspectorUI.SetVisible(true);

        return m_InspectorUI;
    }
}
