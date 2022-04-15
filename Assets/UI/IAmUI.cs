using UnityEngine;
using UnityEngine.UI;

public delegate void OnClose();

public interface IAmUIFor<TComponent>
  where TComponent: Component
{
    TComponent GetComponent();
    Canvas GetCanvas();
    GraphicRaycaster GetRaycaster();
    bool GetVisible();
    void SetVisible(bool visible);
} 
