using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class LifeformMoveState: IState<Lifeform>
{
    private static LifeformMoveState m_Instance = new LifeformMoveState();
    public static LifeformMoveState Instance => m_Instance;

    public string Identifier => "Moving";
    
    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf){}

    public IState<Lifeform> HandleDestinationReached(Lifeform lf, LifeformInterest interest)
    {
        // No interest to interact with at destination
        if(interest == null)
          return LifeformIdleState.Instance;

        // What interest?
        switch(interest.Intent)
        {
            case LifeformIntent.Breed:
              return LifeformBreedState.Instance;
            case LifeformIntent.Interact:
              return LifeformInteractState.Instance;
            default:
              throw new NotImplementedException();
        }
    }

    private IState<Lifeform> PursueInterests(Lifeform lf)
    {
        if(!lf.Interests.Any())
          return null;

        Vector3 lifeformPosition = lf.Navigation.GetPosition();
        float interactionDistance = lf.Navigation.GetInteractionDistance();
        float eyesight = lf.Genetics.GetEyesightDistance();

        IEnumerable<LifeformInterest> interests = lf.Interests.GetInterests();

        for(int i = 0; i < interests.Count(); i++)
        {
            LifeformInterest interest = interests.ElementAt(i);
            if(interest.Object == null) {
                continue;
            }

            Vector3 interestPosition = interest.Object.transform.position;

            // Out of perception?
            float distanceToInterest = Vector3.Distance(lifeformPosition, interestPosition);
            if(distanceToInterest > eyesight) 
            {
                lf.Interests.Remove(interest);
                continue;
            }

            // Out of interaction range?
            if(distanceToInterest > interactionDistance)
            {
                lf.Navigation.LookAt(interestPosition);
                lf.Navigation.SetDestination(interestPosition);
                return LifeformMoveState.Instance;
            }

            // Can interact with interest
            return HandleDestinationReached(lf, interest);
        }

        return null;
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    { 
        lf.DeltaMove();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        // Are we already going somewhere?
        if(lf.Navigation.HasPath())
        {
          if(lf.Navigation.HasReachedDestination())
          {
              lf.Navigation.ResetPath();
              return LifeformInteractState.Instance;
          }
          else
              return null;
        } 

        // Do we have an interest to pursue?
        IState<Lifeform> pursueState = PursueInterests(lf);
        if(pursueState == null)
        {
            return LifeformIdleState.Instance;
        } 
        else
          return pursueState;
    }
}
