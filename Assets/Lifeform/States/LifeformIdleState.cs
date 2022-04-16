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
        lf.Navigation.ResetPath();
    }

    private bool SleepConditions(Lifeform lf)
    {
        return lf.Energy <= lf.Genetics.GetMaxEnergy() * 0.25f;
    }

    private bool EatConditions(Lifeform lf)
    {
        return lf.Hunger <= lf.Genetics.GetMaxHunger() * 0.25f;
    }

    private bool BreedConditions(Lifeform lf)
    {
        return lf.Age >= lf.Genetics.GetMaxAge() * 0.01f
            && Random.value > lf.Genetics.GetBreedRate();
    }

    private bool MoveConditions(Lifeform lf)
    {
        if(lf.Navigation.HasPath() || lf.Interests.Any())
          return true;

        // Wandering
        if(lf.Energy > lf.Genetics.GetMaxEnergy() * 0.8f)
          return true;

        return false;
    }

    // TODO: Move this somewhere...
    private Lifeform FindBreedingPartner(Lifeform lf)
    {
        Lifeform other = null;
        IEnumerable<GameObject> closestLifeforms = lf.Perception.OrderByClosest();

        foreach(GameObject perceivedObject in closestLifeforms)
        {
            if(perceivedObject == null)
              continue;
            if(!perceivedObject.tag.Equals("Player"))
              continue;
            
            Lifeform candidate = perceivedObject.GetComponent<Lifeform>();
            if(candidate == null)
              continue;

            if(!BreedConditions(candidate))
              continue;
  
            other = candidate;
            break;
        }

        return other;
    }

    // Called each frame when in the current state. 
    //  Returns state to transition to, or null to remain in the current state
    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        lf.Navigation.Stop();
        lf.Delta();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        if(EatConditions(lf))
          return LifeformEatState.Instance;

        if(SleepConditions(lf))
          return LifeformSleepState.Instance;

        if(BreedConditions(lf)) 
        {
            Lifeform candidate = FindBreedingPartner(lf);
            if(candidate != null)
            {
                lf.Interests.Add(LifeformIntent.Breed, candidate);
                return LifeformBreedState.Instance;
            }
        }

        if(MoveConditions(lf))
          return LifeformMoveState.Instance;

        return null;
    }
}
