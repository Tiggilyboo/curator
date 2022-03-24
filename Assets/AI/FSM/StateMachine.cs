using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<TComponent>: MonoBehaviour
    where TComponent: Component
{
    [SerializeField]
    private TComponent m_Component;
    [SerializeField]
    private State<TComponent> m_CurrentState;

    public State<TComponent> GetCurrentState() => m_CurrentState;
    public TComponent GetStateComponent() => m_Component;

    public void TransitionTo(State<TComponent> state)
    {
        // TODO: Do we need to check if state belongs to one of the state's transitions?
        m_CurrentState  = state;
    }

    public void UpdateStateMachine() 
    {
        m_CurrentState.UpdateState(this);
    }
}
