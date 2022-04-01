using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/Eating State", fileName = "LifeformEatState")]
public class LifeformEatState: State<Lifeform>
{
    [SerializeField]
    private float m_HungerThresholdInSec = 5.0f;

    private bool DoesLifeformRequireFood(Lifeform lf)
    {
        float rate = lf.Genetics.GetHungerRate();
        float remainingIterations = lf.Hunger / rate;
        float remainingTime = remainingIterations * Time.deltaTime;

        return remainingTime < m_HungerThresholdInSec;
    }

    // Entry condition to this state. Truthy will transition the state machine to this state.
    public override bool EntryCondition(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        bool notEatingAndHungry = (!lf.Eating && DoesLifeformRequireFood(lf));
        if(notEatingAndHungry)
          return true;

        bool eatingAndNotFull = (lf.Eating && lf.Hunger < lf.Genetics.GetMaxHunger());
        
        return eatingAndNotFull;
    }

    // Invoked every frame that the state is active in the state machine
    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        // TODO: Not sure if inventory driven, or location on map (stockpile / foraging?)
        // Should probably do a timer here too...
        lf.Eat(5.0f * Time.deltaTime);
        lf.IncrementAge();
        lf.DecrementEnergy();
    }
}
