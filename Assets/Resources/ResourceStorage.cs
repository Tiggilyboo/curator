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
public sealed class InitialResourcePair
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

    private Dictionary<string, Resource> m_Resources;

    [SerializeField]
    private List<InitialResourcePair> m_InitialResources;

    private void Start()
    {
        if(m_Resources == null)
            m_Resources = new Dictionary<string, Resource>();

        if(m_InitialResources != null)
        {
            foreach(InitialResourcePair pair in m_InitialResources)
            {
                Resource r = pair.Resource;
                r.Quantity = pair.Quantity;
                Add(r);
            }
        }
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
        }

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
                m_Resources.Remove(resource.Identifier);
                OnResourceRemoved?.Invoke(heldResource);
            }
        }
        else
            throw new KeyNotFoundException(resource.Identifier);
    }

    public IEnumerable<Resource> AsEnumerable() => m_Resources.Values;
}
