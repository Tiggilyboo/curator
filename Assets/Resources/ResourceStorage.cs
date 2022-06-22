using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaveResources
{
    public ResourceStorage GetResourceStorage();
}

[Serializable]
public sealed class ResourceQuantityPair
{
    public Resource Resource;
    public int Quantity;
}

public class ResourceStorage: MonoBehaviour
{
    public delegate void ResourceUpdateDelegate(Resource resource);
    public event ResourceUpdateDelegate OnResourceUpdated;
    public event ResourceUpdateDelegate OnResourceRemoved;
    public event ResourceUpdateDelegate OnResourceAdded;

    [SerializeField]
    private Dictionary<string, Resource> m_Resources;

    [SerializeField]
    private List<ResourceQuantityPair> m_InitialResources;

    public void Initialise()
    {
        if(m_InitialResources != null)
        {
            foreach(ResourceQuantityPair pair in m_InitialResources)
            {
                if(pair.Quantity <= 0)
                  continue;

                Resource r = pair.Resource;
                r.Quantity = pair.Quantity;
                Add(r);
            }
        }
    }

    private void Start()
    {
        if(m_Resources == null)
            m_Resources = new Dictionary<string, Resource>();

        Initialise();
    }

    public void Add(Resource resource)
    {
        if(m_Resources.TryGetValue(resource.Identifier, out Resource heldResource)) 
        {
            heldResource.Quantity += resource.Quantity;
            OnResourceUpdated?.Invoke(heldResource);
        } 
        else 
        {
            m_Resources.Add(resource.Identifier, resource);
            OnResourceAdded?.Invoke(resource);
            resource.OnResourceEmpty += HandleResourceRemoved;
        }
    }

    private void HandleResourceRemoved(Resource r) 
    {
        m_Resources.Remove(r.Identifier);
        OnResourceRemoved?.Invoke(r);
        Debug.Log("Resource depleted");
    }

    public bool Has(Resource resource)
    {
        if(m_Resources.TryGetValue(resource.Identifier, out Resource heldResource))
            return heldResource.Quantity >= resource.Quantity;

        return false;
    }

    public void Remove(Resource resource)
    {
        if(m_Resources.TryGetValue(resource.Identifier, out Resource heldResource))
        {
            if(heldResource.Quantity > resource.Quantity)
            {
                heldResource.Quantity -= resource.Quantity;
                OnResourceUpdated?.Invoke(heldResource);
            }
            else 
            {
                HandleResourceRemoved(heldResource);
            }
        }
        else
            throw new KeyNotFoundException(resource.Identifier);
    }

    public IEnumerable<Resource> AsEnumerable() => m_Resources.Values;

    public void Clear()
    {
        for(int i = 0; i < m_Resources.Values.Count; i++)
        {
            Resource r = AsEnumerable().ElementAt(i);
            HandleResourceRemoved(r);
        }
    }
}
