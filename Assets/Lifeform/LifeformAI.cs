using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Lifeform))]
public class LifeformAI : MonoBehaviour
{
    // feels weird / dirty, assuming normally derived from a class somewhere
    LayerMask walkable;


    Lifeform lifeform;
    Interest interest;

    // Start is called before the first frame update
    void Start()
    {
      lifeform = GetComponent<Lifeform>();
      interest = Interest.Idle();
      walkable = LayerMask.GetMask("Walkable");
    }

    GameObject GetWalkableObject() 
    {
      // Could not get this working with walkable layermask with standard Raycast, just was not colliding!
        if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, walkable))
        {
            Debug.Log(hit);
            return hit.collider?.gameObject; 
        }

        return null;
    }

    Vector3 GetRandomPointOnObject(GameObject o)
    {
        var walkableCollider = o.GetComponent<MeshCollider>();
        var walkableBounds = walkableCollider.bounds;
        var randomPoint = new Vector3(
            Random.value * walkableBounds.size.x,
            0.0f, // use terrain height here
            Random.value * walkableBounds.size.z,
        );

        return randomPoint;
    }

    void FindNewInterest()
    {
        var walkableObject = GetWalkableObject();
        if(walkableObject == null) {
          Debug.Log("No walkable object found");
          goto assignIdle;
        }
        var randomPointOnWalkable = GetRandomPointOnObject(walkableObject);
        if(randomPointOnWalkable == null) {
          throw new NullReferenceException();
        }

        interest = Interest.Wander(randomPointOnWalkable);
        return;

assignIdle:
        interest = Interest.Idle();
    }

    void Move(Vector3 destination)
    {
      var speed = 5.0f; // derive from lifeform genetics
      transform.LookAt(destination);
      if(Vector3.Distance(transform.position, destination) <= 2.0f) 
      {
        return;
      }
      transform.Translate(transform.forward * speed * Time.deltaTime);
    }


    // Update is called once per frame
    void Update()
    {
        if(interest == null) 
        {
          FindNewInterest();
        }
        else 
        {
          interest.Dedication -= Time.deltaTime;
          if(interest.Dedication <= 0.0f) {
            interest = null;
            return;
          }
        }

        switch(interest.Intent) {
            case Intent.Idle:
              break;
            case Intent.Wander:
              Move(interest.Point);
              break;
            default:
              throw new NotImplementedException();
        }
    }
}
