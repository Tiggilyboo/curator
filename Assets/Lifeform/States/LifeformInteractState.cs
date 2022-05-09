using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class LifeformInteractState: IState<Lifeform>
{
    private static LifeformInteractState m_Instance = new LifeformInteractState();
    public static LifeformInteractState Instance => m_Instance;

    public string Identifier => "Interacting";

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf){}

    private bool CanLifeformInteractWith(Lifeform lf, Transform t)
    {
        Vector3 lf_pos = lf.Navigation.GetPosition();
        float interactionDistance = lf.Navigation.GetInteractionDistance();
        
        if(Vector3.Distance(lf_pos, t.position) < interactionDistance)
          return true;

        return false;
    }

    private bool InteractWithObject(Lifeform lf, LifeformInterest interest)
    {
        if(!CanLifeformInteractWith(lf, interest.Object.transform))
          return false;

        return true;
    }

    private bool InteractWithLifeform(Lifeform lf, LifeformInterest interest)
    {
        if(!CanLifeformInteractWith(lf, interest.Lifeform.gameObject.transform))
          return false;

        return true;
    }

    private bool TryInteractWithInterest(Lifeform lf)
    {
        foreach(LifeformInterest i in lf.Interests.GetInterests())
        {
            if(i.IsLifeform() && InteractWithLifeform(lf, i))
                return true;

            if(i.IsObject() && InteractWithObject(lf, i))
                return true;
        }

        return false;
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        lf.Delta();
        if(lf.IsDying())
          return LifeformDeadState.Instance;

        if(TryInteractWithInterest(lf))
          return LifeformIdleState.Instance;

        // Unable to interact, pursue further interactions
        return LifeformMoveState.Instance;
    }
}
