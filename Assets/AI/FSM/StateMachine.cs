using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<TComponent>: MonoBehaviour
    where TComponent: Component
{
    private IState<TComponent> m_CurrentState;

    [SerializeField]
    private TComponent m_Component;

    public IState<TComponent> GetCurrentState() => m_CurrentState;
    public TComponent GetStateComponent() => m_Component;

    public bool Is(IState<TComponent> state)
    {
        // TODO: Is this faster than "is typeof(state)" checks?
        return m_CurrentState.Identifier == state.Identifier;
    }

    public void UpdateStateMachine() 
    {
        IState<TComponent> newState = m_CurrentState.UpdateState(m_Component);
        if(newState == null)
            return;
       
        // Transition to newState
        m_CurrentState.OnExit(m_Component);
        m_CurrentState = newState;
        m_CurrentState.OnEntry(m_Component);
    }

    public void Initialize(IState<TComponent> initialState)
    {
        m_CurrentState = initialState;
    }
}
