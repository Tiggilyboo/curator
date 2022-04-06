using UnityEngine;

public class LifeformSleepState: IState<Lifeform>
{
    private static LifeformSleepState m_Instance = new LifeformSleepState();
    public static LifeformSleepState Instance => m_Instance;

    public string Identifier => "Sleeping";

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf){}

    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        float sleepRate = lf.Genetics.GetSleepRate();
        if(lf.Energy + sleepRate >= lf.Genetics.GetMaxEnergy())
          return LifeformIdleState.Instance;

        lf.DeltaAge();
        lf.DeltaHunger(-lf.Genetics.GetHungerRate());
        lf.DeltaEnergy(sleepRate);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        return null;
    }
}
