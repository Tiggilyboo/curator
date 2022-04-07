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
                lf.Interests.Remove(interest);
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

    // Wander: Random point within lieform's perception radius
    private void Wander(Lifeform lf)
    {
        float eyesight = lf.Genetics.GetEyesightDistance();
        float wanderDistance = Random.Range(0, eyesight);
        if(wanderDistance < lf.Navigation.GetInteractionDistance())
          return;

        Vector2 randomCircle = Random.insideUnitCircle;
        Vector3 randomPointInRadius = new Vector3(
            randomCircle.x * wanderDistance,
            0, // TODO: Raycast terrain?
            randomCircle.y * wanderDistance);

        Vector3 wanderPos = lf.Navigation.GetPosition() + randomPointInRadius;
        lf.Navigation.LookAt(wanderPos);
        lf.Navigation.SetDestination(wanderPos);
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    { 
        lf.DeltaAge();
        lf.DeltaHunger(-lf.Genetics.GetHungerRate());

        // Further incur penalty for velocity
        Vector3 velocity = lf.Navigation.GetVelocity();
        float moveRate = lf.Genetics.GetMoveRate();
        lf.DeltaEnergy(-moveRate * velocity.sqrMagnitude * 0.001f);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        // Are we already going somewhere?
        if(lf.Navigation.HasPath())
        {
          if(lf.Navigation.HasReachedDestination())
          {
            lf.Navigation.ResetPath();
            return LifeformIdleState.Instance;
          }
          else
            return null;
        } 

        // Do we have an interest to pursue?
        IState<Lifeform> pursueState = PursueInterests(lf);
        if(pursueState == null)
        {
            if(!lf.Navigation.HasPath())
              Wander(lf);

            return null;
        } 
        else if(pursueState == LifeformMoveState.Instance)
        {
            return null;
        }
        else
          return pursueState;
    }
}
