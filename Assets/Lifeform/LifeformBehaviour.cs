using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LifeformBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_Walkable;
    [SerializeField]
    private Lifeform m_Lifeform;
    [SerializeField]
    private Interest m_Interest;
    [SerializeField]
    private float m_Dedication;

    public Interest CurrentInterest => m_Interest;

    private GameObject GetWalkableObjectInRange() 
    {
        float distance = m_Lifeform.GetEyesightDistance(); 
        if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, distance, m_Walkable))
        {
            return hit.collider?.gameObject; 
        }

        Debug.Log(string.Format("Unable to find walkable object within range: {0}", distance)); 
        return null;
    }

    private Vector3 GetRandomPointOnObject(GameObject o)
    {
        MeshCollider walkableCollider = o.GetComponent<MeshCollider>();
        Bounds walkableBounds = walkableCollider.bounds;
        Vector3 randomPoint = new Vector3(
            Random.value * walkableBounds.size.x,
            0.0f, // use terrain height here
            Random.value * walkableBounds.size.z
        );

        return randomPoint;
    }

    private void FindNewInterest()
    {
        GameObject walkableObject = GetWalkableObjectInRange();
        if(walkableObject == null) 
        {
            m_Interest = new Interest(Intent.Idle);
            m_Dedication = 5.0f;
            return;
        }

        Vector3 wanderDestination = GetRandomPointOnObject(walkableObject);
        if(wanderDestination == null) 
        {
            throw new NullReferenceException();
        }

        Debug.Log(string.Format("wants to wander to {0}", wanderDestination.ToString()));
        m_Interest = new Interest(Intent.Wander, wanderDestination);
        m_Dedication = 10.0f;
    }

    private void Move(Vector3 destination)
    {
      // Check if we have reached the destination (or thereabouts), and reset our interest
      // TODO: Use proper radius. Should be resolved anyways when we move to NavMesh
      if(Vector3.Distance(transform.position, destination) <= 2.0f) 
      {
          m_Interest = new Interest(Intent.Idle);
          m_Dedication = 5.0f;
          return;
      }

      var speed = 5.0f; // derive from lifeform genetics
      transform.LookAt(destination);
      transform.Translate(transform.forward * speed * Time.deltaTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Interest = new Interest(Intent.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        m_Dedication -= Time.deltaTime;
        if(m_Dedication <= 0.0f) 
        {
          FindNewInterest();
          return;
        }

        // TODO: Implement FSM
        switch(m_Interest.Intent) 
        {
            case Intent.Idle:
              break;
            case Intent.Wander:
              Move(m_Interest.Point);
              break;
            default:
              throw new NotImplementedException();
        }
    }
}
