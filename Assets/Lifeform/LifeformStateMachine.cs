using UnityEngine;
using UnityEngine.AI;

public class LifeformStateMachine: StateMachine<Lifeform>
{
    [SerializeField]
    private bool m_Active = true;

    [SerializeField]
    private string CurrentStateIdentifier => GetCurrentState().Identifier;

    public void Stop()
    {
        m_Active = false;
    }

    public void Update() 
    {
        if(m_Active)
            base.UpdateStateMachine(); 
    }

    public void Start()
    {
        base.Initialize(LifeformIdleState.Instance);
    }
}
