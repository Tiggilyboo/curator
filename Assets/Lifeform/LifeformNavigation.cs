using UnityEngine;
using UnityEngine.AI;

public class LifeformNavigation: MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent m_NavAgent;

    [SerializeField]
    private Lifeform m_Lifeform;
    
    public NavMeshAgent GetNavMeshAgent() => m_NavAgent;
    public Vector3 GetPosition() => transform.position;
    public void LookAt(Vector3 point) => transform.LookAt(point);
    public Vector3 GetDestination() => m_NavAgent.destination;
    public float GetDistanceRemaining() => m_NavAgent.remainingDistance;
    public bool HasPath() => m_NavAgent.hasPath;

    public void ResetPath() 
    {
        m_NavAgent.ResetPath();
    }

    public void SetDestination(Vector3 destination)
    {
        m_NavAgent.SetDestination(destination);
    }
   
    public void Start()
    {
        m_NavAgent.speed = m_Lifeform.GetMoveRate();
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
