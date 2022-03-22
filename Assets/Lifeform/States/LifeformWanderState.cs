using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "AI/FSM/Lifeform/Wander State", fileName = "LifeformWanderState")]
public class LifeformWanderState: State<Lifeform>
{
    [SerializeField]
    private float m_MinWaypointRadius = 2.0f;

    [SerializeField]
    private LayerMask m_Walkable;

    // TODO: NavMesh
    private Nullable<Vector3> m_Destination = null;

    private bool HasDestination() 
    {
        return m_Destination != null;
    }

    private bool HasReachedDestination(Lifeform lf) 
    {
        return m_Destination.HasValue
          && Vector3.Distance(lf.GetPosition(), m_Destination.Value) < m_MinWaypointRadius;
    }

    private Nullable<Vector3> GetRandomWalkablePointInRange(Lifeform lf, float range)
    {
        Vector3 radius = Random.insideUnitCircle * range;
        if(Physics.Raycast(lf.GetPosition() + radius + Vector3.up, Vector3.down, out RaycastHit hit, range, m_Walkable))
        {
           return hit.point;
        }

        return null;
    }

    private bool SetDestinationInRange(Lifeform lf, float range)
    {
        m_Destination = GetRandomWalkablePointInRange(lf, range);
        
        return m_Destination.HasValue;
    }

    public override bool EntryCondition(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        return (!lf.Moving || HasDestination())
            && !lf.Eating
            && !lf.Sleeping
            && lf.Energy > 0
            && lf.Hunger > 0;
    }

    public override void StateEffect(StateMachine<Lifeform> s)
    {
        Lifeform lf = s.GetStateComponent();

        if(HasDestination())
        {
            var destination = m_Destination.Value;
            lf.LookAt(destination);
            lf.Translate(lf.GetForward() * lf.GetMoveRate() * Time.deltaTime);
        } 
        else if(SetDestinationInRange(lf, lf.GetEyesightDistance())) 
        {
            lf.Move();
        } 
        if(HasReachedDestination(lf))
        {
            lf.Stop();
            m_Destination = null;
            return;
        }

        lf.DecrementEnergy();
        lf.DecrementHunger();
        lf.IncrementAge();
    }
}
