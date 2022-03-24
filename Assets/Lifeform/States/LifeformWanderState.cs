using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/Wander State", fileName = "LifeformWanderState")]
public class LifeformWanderState: State<Lifeform>
{
    [SerializeField]
    private float m_MinDestinationDistance = 1.0f;
    [SerializeField]
    private LayerMask m_Walkable;

    private Nullable<Vector3> GetRandomWalkablePointInRange(Lifeform lf, float range)
    {
        Vector3 randomWithinRadius = new Vector3(
            range * (-1.0f + Random.value * 2.0f),
            1.0f, // Something above the main walkable object
            range * (-1.0f + Random.value * 2.0f));

        Vector3 rayOrigin = lf.GetNavigation().GetPosition() + randomWithinRadius;
        
        if(Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, range, m_Walkable))
        {
           return hit.point;
        }

        return null;
    }

    private bool SetDestinationInRange(Lifeform lf, float range)
    {
        Nullable<Vector3> nextDestination = GetRandomWalkablePointInRange(lf, range);

        if(nextDestination.HasValue) 
        {
            lf.GetNavigation().SetDestination(nextDestination.Value);
        }

        return nextDestination.HasValue;
    }

    public override bool EntryCondition(StateMachine<Lifeform> stateMachine)
    {
        Lifeform lf = stateMachine.GetStateComponent();
        LifeformNavigation nav = lf.GetNavigation();

        return (!lf.Moving || nav.HasPath())
            && !lf.Eating
            && !lf.Sleeping;
    }

    public override void StateEffect(StateMachine<Lifeform> stateMachine)
    {
        Lifeform lf = stateMachine.GetStateComponent();
        LifeformNavigation nav = lf.GetNavigation();

        if(!nav.HasPath())
        {
            if(SetDestinationInRange(lf, lf.GetEyesightDistance())) 
            {
                Vector3 dest = nav.GetDestination();
                nav.LookAt(dest);
            } 
        }
        else if(nav.GetDistanceRemaining() <= m_MinDestinationDistance)
        {
            nav.ResetPath();
            return;
        } 
        else
        {
            lf.Move();
        }
        
        lf.DecrementEnergy();
        lf.DecrementHunger();
        lf.IncrementAge();
    }
}
