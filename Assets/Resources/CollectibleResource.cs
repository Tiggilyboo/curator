using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleResource: MonoBehaviour, IInteractable
{
    private float m_LastReplentishmentTime;
    private ResourceStorage m_Resources;

    [SerializeField]
    private List<ResourceQuantityPair> m_Replentishment;
    [SerializeField]
    private float m_ReplentishRateInSec = 5f;
    [SerializeField]
    private bool m_DestroyOnDepletion = false;

    private void ReplentishResources()
    {
        foreach(ResourceQuantityPair pair in m_Replentishment)
        {
            Resource r = pair.Resource;
            r.Quantity = pair.Quantity;
            m_Resources.Add(r);
        }
    }
    
    public bool Interact(Lifeform lf)
    {
        foreach(Resource r in m_Resources.AsEnumerable())
            lf.Inventory.Add(r);

        m_Resources.Clear();

        if(m_DestroyOnDepletion)
            GameObject.Destroy(gameObject);

        return true;
    }

    private void Update()
    { 
        if(Time.realtimeSinceStartup < m_LastReplentishmentTime + m_ReplentishRateInSec)
            return;

        ReplentishResources();
    }

    private void Start()
    {
        m_Resources = gameObject.AddComponent<ResourceStorage>();
        m_LastReplentishmentTime = Time.realtimeSinceStartup;
        ReplentishResources();
    }
}
