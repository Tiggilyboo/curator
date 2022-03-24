using UnityEngine;
using UnityEngine.AI;

public class LifeformStateMachine: StateMachine<Lifeform>
{
    public void Update() 
    {
        base.UpdateStateMachine(); 
    }
}
