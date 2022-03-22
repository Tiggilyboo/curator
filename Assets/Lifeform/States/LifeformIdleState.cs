using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/Idle State", fileName = "LifeformIdleState")]
public class LifeformIdleState: State<Lifeform>
{
    public override bool EntryCondition(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        return lf.Energy > 0
            && lf.Hunger > 0;
    }

    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();
        lf.Stop();

        lf.DecrementEnergy();
        lf.DecrementHunger();
        lf.IncrementAge();
    }
}
