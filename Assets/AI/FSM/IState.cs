using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<TComponent>
    where TComponent: Component
{
    string Identifier { get; }

    void OnEntry(TComponent s);
    void OnExit(TComponent s);

    IState<TComponent> UpdateState(TComponent component);
}
