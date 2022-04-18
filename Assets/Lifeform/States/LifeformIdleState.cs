using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LifeformIdleState: IState<Lifeform>
{
    private static LifeformIdleState m_Instance = new LifeformIdleState();
    public static LifeformIdleState Instance => m_Instance;

    public string Identifier => "Idle";

    public void OnExit(Lifeform lf) {}
    public void OnEntry(Lifeform lf) 
    {
        lf.Navigation.Stop();
        lf.Navigation.ResetPath();
    }

    // Called each frame when in the current state. 
    //  Returns state to transition to, or null to remain in the current state
    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        lf.Delta();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        if(lf.EatConditions())
          return LifeformEatState.Instance;

        if(lf.SleepConditions())
          return LifeformSleepState.Instance;

        if(lf.BreedConditions()) 
            return LifeformBreedState.Instance;

        if(lf.WanderConditions()) 
          return LifeformWanderState.Instance;

        return null;
    }
}
