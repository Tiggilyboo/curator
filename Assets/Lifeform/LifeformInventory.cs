using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeformInventory : MonoBehaviour, IHaveResources
{
    [SerializeField]
    private ResourceStorage m_Storage;
    public ResourceStorage GetResourceStorage() => m_Storage;

    public Resource GetFirstEdibleFromStorage()
    {
        foreach(Resource r in m_Storage.AsEnumerable())
        {
            if(!r.IsEdible)
              continue;

            return r;
        }

        return null;
    }

    public bool TryToEat()
    {
        Resource edible = GetFirstEdibleFromStorage();
        if(edible == null)
          return false;

        edible.Quantity--;
        return true;
    }
}
