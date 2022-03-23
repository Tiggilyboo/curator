using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/Sleep State", fileName = "LifeformSleepState")]
public class LifeformSleepState: State<Lifeform>
{   
    [SerializeField]
    private float m_SleepThresholdInSec = 5.0f;

    private bool DoesLifeformRequireSleep(Lifeform lf) 
    {
        float rate = lf.GetEnergyRate();
        float remainingIterations = lf.Energy / rate;
        float remainingTime = remainingIterations * Time.deltaTime;

        return remainingTime < m_SleepThresholdInSec;
    }

    public override bool EntryCondition(StateMachine<Lifeform> s) 
    {
        Lifeform lf = s.GetStateComponent();

        bool sleepingAndNotFullySlept = (lf.Sleeping && lf.Energy < lf.GetMaxEnergy());
        if(sleepingAndNotFullySlept)
          return true;

        bool notSleepingAndLacksEnergy = (!lf.Sleeping && DoesLifeformRequireSleep(lf));

        return notSleepingAndLacksEnergy;
    }

    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        lf.Sleep();
        lf.DecrementHunger();
        lf.IncrementAge();
    }
}
