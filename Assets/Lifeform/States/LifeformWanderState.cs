using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LifeformWanderState: IState<Lifeform>
{
    // When the distance * move rate * multiplier time has elapsed, we give up wandering
    private const float WaitTimeToArriveMultiplier = 10f;
    private static LifeformWanderState m_Instance = new LifeformWanderState();
    public static LifeformWanderState Instance => m_Instance;

    public string Identifier => "Wandering";

    // Invoked when the state machine enters this state
    public void OnEntry(Lifeform lf){}

    // Invoked when the state machine exits this state
    public void OnExit(Lifeform lf){}

    // Invoked every frame that the state is active in the state machine
    //    Returns null to remain in the current state, or the state to transition to.
    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        lf.DeltaMove();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        float eyesight = lf.Genetics.GetEyesightDistance();
        float interactionDistance = lf.Navigation.GetInteractionDistance();
        if(lf.Navigation.HasPath())
        {
            if(lf.Navigation.GetRemainingDistance() > eyesight)
                return LifeformIdleState.Instance;

            if(lf.Navigation.GetRemainingDistance() <= interactionDistance)
                return LifeformIdleState.Instance;

            // TODO: Give up if inaccessible

            // Continue wandering
            return null;
        }

        // How far to wander?
        float wanderDistance = Random.Range(0, eyesight);
        if(wanderDistance < interactionDistance)
          return null;

        // Wander to a new location within the distance
        Vector2 randomCircle = Random.insideUnitCircle;
        Vector3 randomPointInRadius = new Vector3(
            randomCircle.x * wanderDistance,
            0, // TODO: Raycast terrain?
            randomCircle.y * wanderDistance);

        Vector3 wanderPos = lf.Navigation.GetPosition() + randomPointInRadius;
        lf.Navigation.LookAt(wanderPos);
        lf.Navigation.SetDestination(wanderPos);

        return null;
    }
}
