using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class State<TComponent>: ScriptableObject
    where TComponent: Component
{
    [SerializeField]
    private string m_Identifier;
    [SerializeField]
    private List<StateEffect<TComponent>> m_StateEffects = new List<StateEffect<TComponent>>();
    
    public string Identifier => m_Identifier;

    public void UpdateEffects(StateMachine<TComponent> s)
    {
        foreach(var effect in m_StateEffects)
        {
            effect.Invoke(s);
        }
    }
}

public abstract class StateEffect<TComponent>: ScriptableObject
    where TComponent: Component
{
    public abstract void Invoke(StateMachine<TComponent> s);
}


public abstract class StateCondition<TComponent>: ScriptableObject
    where TComponent: Component
{
    public abstract bool Condition(StateMachine<TComponent> s);
}

[Serializable]
public class StateTransition<TComponent>: ScriptableObject
    where TComponent: Component
{
    [SerializeField]
    private State<TComponent> m_PositiveState;
    [SerializeField]
    private State<TComponent> m_NegativeState;
    [SerializeField]
    private StateCondition<TComponent> m_Condition;

    public State<TComponent> PositiveOutcome => m_PositiveState;
    public State<TComponent> NegativeOutcome => m_NegativeState;
    public bool Condition(StateMachine<TComponent> s) => m_Condition.Condition(s);
}

public abstract class StateMachine<TComponent>: MonoBehaviour
  where TComponent: Component
{
    [SerializeField]
    private State<TComponent> m_CurrentState;
    [SerializeField]
    private TComponent m_Component;
    [SerializeField]
    private StateTransition<TComponent>[] m_Transitions;

    public StateTransition<TComponent>[] Transitions => m_Transitions;

    public State<TComponent> GetCurrentState() => m_CurrentState;
    public TComponent GetStateMachineComponent() => m_Component;

    public void Update()
    {
        if(m_CurrentState != null) 
        {
            m_CurrentState.UpdateEffects(this);
        }

        foreach(var transition in m_Transitions)
        {
           if(transition.Condition(this)) {
              TransitionTo(transition.PositiveOutcome);
           } else {
              TransitionTo(transition.NegativeOutcome);
           }
        }
    }

    private void TransitionTo(State<TComponent> state) 
    {
        Debug.Log(string.Format("Transitioning to {0}", state.Identifier));
        m_CurrentState = state;
    }
}
