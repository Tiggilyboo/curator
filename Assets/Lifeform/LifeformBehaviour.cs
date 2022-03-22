using UnityEngine;

public class LifeformBehaviour: StateMachine<Lifeform>
{
    public void Update() 
    {
        base.UpdateStateMachine(); 
    }
}
