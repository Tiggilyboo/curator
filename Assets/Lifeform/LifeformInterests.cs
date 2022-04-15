using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum LifeformIntent
{
    Breed,
}

[Serializable]
public class LifeformInterest
{
    [SerializeField]
    private GameObject m_Object;
    [SerializeField]
    private LifeformIntent m_Intent;
    [SerializeField]
    private Lifeform m_Lifeform;

    public GameObject Object => m_Object;
    public LifeformIntent Intent => m_Intent;
    public Lifeform Lifeform => m_Lifeform;

    public LifeformInterest(LifeformIntent intent, GameObject gameObject)
    {
        m_Object = gameObject;
        m_Intent = intent;
    }

    public LifeformInterest(LifeformIntent intent, Lifeform lifeform)
        : this(intent, lifeform.gameObject)
    {
        m_Lifeform = lifeform;
    }
}

public class LifeformInterests: MonoBehaviour
{
    [SerializeField]
    private List<LifeformInterest> m_Interests;

    [SerializeField]
    private Lifeform m_Lifeform;

    public IEnumerable<LifeformInterest> GetInterests() =>  m_Interests;

    public bool Any() => m_Interests.Any();

    public void Add(LifeformIntent intent, Lifeform lifeform)
    {
        m_Interests.Add(new LifeformInterest(intent, lifeform));
    }

    public void Remove(LifeformIntent intent, GameObject gameObject)
    {
        int id = gameObject.GetInstanceID();

        // TODO: Store / fetch as a Dictionary? Depends how many big ideas this guy has...
        for(int i = 0; i < m_Interests.Count; i++)
        {
            LifeformInterest interest = m_Interests[i];

            if(interest.Object.GetInstanceID() == id && interest.Intent == intent)
            {
                m_Interests.RemoveAt(i);
                return;
            }
        }
    }

    public void RemoveAllWith(LifeformIntent intent)
    {
        for(int i = 0; i < m_Interests.Count; i++)
        {
            LifeformInterest interest = m_Interests[i];

            if(m_Interests[i].Intent != intent)
              continue;

            m_Interests.RemoveAt(i);
        }
    }

    public void Remove(LifeformInterest interest)
    {
        for(int i = 0; i < m_Interests.Count; i++)
        {
            if(interest.Equals(m_Interests[i]))
            {
                m_Interests.RemoveAt(i);
                return;
            }
        }
    }

    void Start()
    {
        m_Interests = new List<LifeformInterest>();
    }
}
