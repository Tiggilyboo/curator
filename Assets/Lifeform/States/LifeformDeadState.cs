using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/New Dead State", fileName = "LifeformDeadState")]
public class LifeformDeadState: State<Lifeform>
{
    // Entry condition into this state. True will transition the state machine to this state.
    public override bool EntryCondition(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        return lf.Energy <= 0.0f
            || lf.Hunger <= 0.0f
            || lf.Age >= lf.Genetics.GetMaxAge();
    }

    // Invoked every frame that the state is active in the state machine
    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        lf.Die();
    }
}
