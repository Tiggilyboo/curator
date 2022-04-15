using UnityEngine;
using UnityEngine.UI;

public delegate void OnClose();

public interface IAmUIFor<TComponent>: IAmUI
  where TComponent: Component
{
    TComponent GetComponent();
} 

public interface IAmUI
{
    Canvas GetCanvas();
    GraphicRaycaster GetRaycaster();
    bool GetVisible();
    void SetVisible(bool visible);
}
