using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeformBreedState: IState<Lifeform>
{
    private static LifeformBreedState m_Instance = new LifeformBreedState();
    public static LifeformBreedState Instance => m_Instance;

    public string Identifier => "Breeding";

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf){}

    private Lifeform GetBreedingInterest(Lifeform lf)
    {
        foreach(LifeformInterest i in lf.Interests.GetInterests())
        {
            if(i.Intent == LifeformIntent.Breed)
                return i.Lifeform;
        }

        return null;
    }
    
    private Lifeform FindBreedingPartner(Lifeform lf)
    {
        Lifeform other = null;
        IEnumerable<GameObject> closestLifeforms = lf.Perception.OrderByClosest();

        foreach(GameObject perceivedObject in closestLifeforms)
        {
            if(perceivedObject == null)
              continue;
            if(!perceivedObject.tag.Equals("Lifeform"))
              continue;
            
            Lifeform candidate = perceivedObject.GetComponent<Lifeform>();
            if(candidate == null)
              continue;

            if(!candidate.BreedConditions())
              continue;
  
            other = candidate;
            break;
        }

        return other;
    }

    private bool TryBreed(Lifeform lf, Lifeform other)
    {
        if(!lf.BreedConditions() || other.BreedConditions())
        {
            lf.Interests.RemoveAllWith(LifeformIntent.Breed);
            return false;
        }

        float chance = Random.value;
        if(chance > lf.Genetics.GetBreedRate()
            && chance > other.Genetics.GetBreedRate())
        {
            lf.Interests.RemoveAllWith(LifeformIntent.Breed);
            return false;
        }

        other.Navigation.Stop();
        other.Navigation.ResetPath();
        other.SetEnergy(0.1f);

        lf.Navigation.Stop();
        lf.Navigation.ResetPath();
        lf.Breed(other);
        lf.SetEnergy(0.1f);
        lf.Interests.RemoveAllWith(LifeformIntent.Breed);

        return true;
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        lf.Delta();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        Lifeform candidate = GetBreedingInterest(lf);

        // No breeding interest found...
        if(candidate == null)
        {
            candidate = FindBreedingPartner(lf);

            // No breeding partner in range, wander around
            if(candidate == null)
              return LifeformWanderState.Instance;

            // Are we already pursuing them?
            if(!lf.Navigation.HasPath())
            { 
                lf.Navigation.SetDestination(candidate.Navigation.GetPosition());
                lf.Interests.Add(LifeformIntent.Breed, candidate);

                return LifeformMoveState.Instance;
            }
        }

        // Found interest, are we in range?
        float dist = Vector3.Distance(lf.Navigation.GetPosition(), candidate.Navigation.GetPosition());
        
        // Within range
        if(dist < lf.Navigation.GetInteractionDistance())
        {
            if(TryBreed(lf, candidate))
              return LifeformSleepState.Instance;
        }

        // Continue pursuing
        lf.Navigation.SetDestination(candidate.Navigation.GetPosition());

        return LifeformMoveState.Instance;
    }
}
