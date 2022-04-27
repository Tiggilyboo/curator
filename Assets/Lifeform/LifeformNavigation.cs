using UnityEngine;
using UnityEngine.AI;

public class LifeformNavigation: MonoBehaviour
{
    // Multiplier * MoveRate = Speed
    [SerializeField]
    private float m_MoveSpeedMultiplier = 10f;
    [SerializeField]
    private NavMeshAgent m_NavAgent;

    [SerializeField]
    private Lifeform m_Lifeform;

    public NavMeshAgent GetNavMeshAgent() => m_NavAgent;
    public Vector3 GetPosition() => transform.position;
    public Vector3 GetDestination() => m_NavAgent.destination;
    public Vector3 GetVelocity() => m_NavAgent.velocity;
    public float GetRemainingDistance() => m_NavAgent.remainingDistance;
    public float GetMoveSpeed() => m_NavAgent.speed;
    public bool HasPath() => m_NavAgent.hasPath;
    public void LookAt(Vector3 point) => transform.LookAt(point);

    public delegate void OnDestinationReachedDelegate(Lifeform lf);
    public event OnDestinationReachedDelegate OnDestinationReached;

    public void ResetPath() 
    {
        m_NavAgent.ResetPath();
    }

    public void Stop()
    {
        m_NavAgent.SetDestination(GetPosition());
    }

    public void SetDestination(Vector3 destination)
    {
        m_NavAgent.SetDestination(destination);
    }
   
    public void SetMoveRate(float moveRate)
    {
        m_NavAgent.speed = m_MoveSpeedMultiplier * moveRate;
    }

    public bool CanNavigate() 
    {
        return (m_NavAgent.isOnNavMesh || m_NavAgent.isOnOffMeshLink);
    }

    public float GetInteractionDistance()
    {
        return m_NavAgent.radius * 2.0f;
    }

    public bool HasReachedDestination()
    {
        return HasPath() && GetRemainingDistance() <= GetInteractionDistance();
    }

    public void OnDrawGizmos()
    {
        if(HasPath()) 
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(GetPosition(), GetDestination());
        }
    }

    private void Update()
    {
        if(CanNavigate() && HasReachedDestination())
        {
            m_NavAgent.ResetPath();
            OnDestinationReached?.Invoke(m_Lifeform);
        }
    }
}
