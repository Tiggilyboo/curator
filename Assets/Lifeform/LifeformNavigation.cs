using UnityEngine;
using UnityEngine.AI;

public class LifeformNavigation: MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent m_NavAgent;

    public NavMeshAgent GetNavMeshAgent() => m_NavAgent;
    public Vector3 GetPosition() => transform.position;
    public Vector3 GetDestination() => m_NavAgent.destination;
    public float GetDistanceRemaining() => m_NavAgent.remainingDistance;
    public bool HasPath() => m_NavAgent.hasPath;
    public void LookAt(Vector3 point) => transform.LookAt(point);

    public void ResetPath() 
    {
        if(!gameObject.activeSelf)
            return;

        m_NavAgent.ResetPath();
    }

    public void SetDestination(Vector3 destination)
    {
        if(!gameObject.activeSelf)
            return;

        m_NavAgent.SetDestination(destination);
    }
   
    public void SetMoveRate(float moveRate)
    {
        m_NavAgent.speed = moveRate;
    }

    public bool CanNavigate() 
    {
        return m_NavAgent.isOnNavMesh || m_NavAgent.isOnOffMeshLink;
    }

    public void OnDrawGizmos()
    {
        if(HasPath()) 
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(GetPosition(), GetDestination());
        }
    }
}
