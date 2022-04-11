using UnityEngine;
using UnityEngine.UI;

public interface IAmUIFor<TComponent>
    where TComponent: Component
{
    TComponent GetComponent();
    Canvas GetCanvas();
    bool GetVisible();
    void SetVisible(bool visible);
} 
