using UnityEngine;

public class LifeformDeadState: IState<Lifeform>
{
    public static LifeformDeadState m_Instance = new LifeformDeadState();
    public static LifeformDeadState Instance => m_Instance;

    public string Identifier => "Dead";

    public void OnEntry(Lifeform lf){}
    public void OnExit(Lifeform lf){}

    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        // Ensure we no longer process the state machine (we are destroying the lifeform!)
        lf.StateMachine.Stop();

        // Super smelly pile.
        // TODO: Stijn, how do I ensure the OnTriggerExit is called for destroyed objects?
        Rigidbody rb = lf.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.WakeUp();
        GameObject.Destroy(lf.gameObject, 0.1f);

        return null;
    }
}
