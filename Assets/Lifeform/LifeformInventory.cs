using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeformInventory : ResourceStorage, IHaveResources
{
    [SerializeField]
    private Lifeform m_Lifeform;

    [SerializeField]
    public ResourceStorage GetResourceStorage() => this;

    public Resource GetFirstEdibleFromStorage()
    {
        foreach(Resource r in base.AsEnumerable())
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

        edible.Interact(m_Lifeform);

        return true;
    }
}
