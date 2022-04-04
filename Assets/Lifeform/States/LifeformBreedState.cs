using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/New Breed State", fileName = "LifeformBreedState")]
public class LifeformBreedState: State<Lifeform>
{
    // TODO: Where do we access tags from in Unity?
    const string TAG_PLAYER = "Player";

    private float GetBreedingAge(Lifeform lf)
    {
        return lf.Genetics.GetMaxAge() / 5;
    }

    public bool CanBreed(Lifeform lf)
    {
        return lf != null
            && !lf.Dead 
            && lf.Age >= GetBreedingAge(lf);
    }

    public Lifeform GetLifeformInRangeOf(Lifeform lf)
    {
        if(lf == null)
          return null;

        foreach(GameObject g in lf.Perception.OrderByClosest())
        {
            if(g == null || !g.tag.Equals(TAG_PLAYER))
              continue;

            Lifeform other = g.GetComponent<Lifeform>();
            if(other == null)
              continue;

            if(!CanBreed(other))
              continue;

            return other;
        }

        return null;
    }

    // Entry condition into this state. True will transition the state machine to this state.
    public override bool EntryCondition(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent(); 

        if(lf.Dead || lf.Age < GetBreedingAge(lf))
          return false;
        
        Lifeform other = null;
        IEnumerable<LifeformInterest> breedingInterests = lf.Interests
          .GetInterests()
          .Where(p => p.Lifeform != null && p.Intent == LifeformIntent.Breed);
        
        if(!breedingInterests.Any()) 
        {
            other = GetLifeformInRangeOf(lf);
            if(other != null)
              lf.Interests.AddInterest(LifeformIntent.Breed, other);
        }
        else
        {
            foreach(LifeformInterest interest in breedingInterests)
            {
                if(interest.Lifeform == null)
                  lf.Interests.RemoveInterest(interest);
            }
        }

        if(other == null)
          return false;

        return true;
    }

    // Invoked every frame that the state is active in the state machine
    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();
        LifeformInterest breedingInterest = lf.Interests.GetInterests()
          .FirstOrDefault(i => i.Intent == LifeformIntent.Breed);

        if(breedingInterest == null)
          return;

        if(breedingInterest.Lifeform == null)
        {
            lf.Interests.RemoveInterest(breedingInterest);
            return;
        }

        // TODO: Pursue interest maybe as a seperate state?
        void ChaseTarget()
        {
            lf.Navigation.SetDestination(breedingInterest.Object.transform.position);
            lf.DecrementEnergy();
            lf.DecrementHunger();
            lf.IncrementAge();
            return;
        }

        bool isPerceived = lf.Perception.InPerception(breedingInterest.Object);
        if(!isPerceived) 
        {
            float distance = Vector3.Distance(lf.transform.position, breedingInterest.Object.transform.position);
            if(distance < 2.0)
            {
                lf.Breed(breedingInterest.Lifeform);
                lf.Interests.RemoveInterestsWithIntent(LifeformIntent.Breed);
                return;
            }
        }

        ChaseTarget();
    }
}
