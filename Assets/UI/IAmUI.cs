using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void OnClose();
public delegate void OnPointer(PointerEventData pointer);

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
