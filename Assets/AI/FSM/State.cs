using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<TComponent>: ScriptableObject
    where TComponent: Component
{
    [SerializeField]
    private string m_Identifier;
    [SerializeField]
    private State<TComponent>[] m_Transitions;

    public State<TComponent>[] Transitions => m_Transitions;
    public string Identifier => m_Identifier;

    public abstract bool EntryCondition(StateMachine<TComponent> s);

    public abstract void StateEffect(StateMachine<TComponent> s);
    
    public void UpdateState(StateMachine<TComponent> s)
    {
        StateEffect(s);

        foreach(State<TComponent> state in m_Transitions)
        {
            if(!state.EntryCondition(s))
              continue;

            if(state.Equals(this))
              return; 

            s.TransitionTo(state);
            return;
        }
    }
}
