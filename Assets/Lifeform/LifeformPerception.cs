using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keep track of other game objects within the lifeform's eyesight distance
public class LifeformPerception : MonoBehaviour
{
    public delegate void OnPerceptionChangedDelegate(Lifeform self, bool added, GameObject perceivedObject);
    public OnPerceptionChangedDelegate OnPerceptionChanged;

    [SerializeField]
    private Lifeform m_Lifeform;
    
    [SerializeField]
    private SphereCollider m_PerceptionCollider;

    private HashSet<int> m_ObjectInstances;

    [SerializeField]
    private List<GameObject> m_ObjectsInPerception;

    public void Start() 
    {
        m_ObjectInstances = new HashSet<int>();
        m_ObjectsInPerception = new List<GameObject>();

        m_PerceptionCollider.radius = m_Lifeform.Genetics.GetEyesightDistance();
        m_PerceptionCollider.isTrigger = false;
    }

    void OnTriggerEnter(Collider other) 
    {
        int id = other.gameObject.GetInstanceID();
        if(m_ObjectInstances.Contains(id))
          return;

        m_ObjectsInPerception.Add(other.gameObject);
        m_ObjectInstances.Add(id);
        if(OnPerceptionChanged != null)
          OnPerceptionChanged.Invoke(m_Lifeform, true, other.gameObject);
    }

    void OnTriggerExit(Collider other) 
    {
        int id = other.gameObject.GetInstanceID();
        if(!m_ObjectInstances.Contains(id))
          return;

        m_ObjectsInPerception.Remove(other.gameObject);
        m_ObjectInstances.Remove(id);
        if(OnPerceptionChanged != null)
          OnPerceptionChanged.Invoke(m_Lifeform, false, other.gameObject);
    }

    public IEnumerable<GameObject> OrderByClosest() 
    {
      if(m_ObjectInstances.Count == 0)
        return new GameObject[]{};

      return m_ObjectsInPerception
          .OrderBy(o => o == null 
              ? -1.0 
              : (o.transform.position - transform.position).sqrMagnitude);
    }

    public bool InPerception(GameObject gameObject)
    {
        int id = gameObject.GetInstanceID();
        return m_ObjectInstances.Contains(id);
    }
}

