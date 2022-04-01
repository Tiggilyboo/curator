using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/Idle State", fileName = "LifeformIdleState")]
public class LifeformIdleState: State<Lifeform>
{
    public override bool EntryCondition(StateMachine<Lifeform> s)
    {
        return true;
    }

    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        lf.Navigation.ResetPath();
        lf.Stop();
        lf.DecrementEnergy();
        lf.DecrementHunger();
        lf.IncrementAge();
    }
}
