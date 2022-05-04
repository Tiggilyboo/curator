using UnityEngine;

public class LifeformSleepState: IState<Lifeform>
{
    private static LifeformSleepState m_Instance = new LifeformSleepState();
    public static LifeformSleepState Instance => m_Instance;

    public string Identifier => "Sleeping";

    private const float HungerHibernationRate = 0.25f;

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf)
    {
        lf.Navigation.ResetPath();
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    {
        float sleepRate = lf.Genetics.GetSleepRate();
        if(lf.Energy + sleepRate >= lf.Genetics.GetMaxEnergy())
          return LifeformIdleState.Instance;

        lf.Navigation.Stop();
        lf.DeltaAge();
        lf.DeltaHunger(-lf.Genetics.GetHungerRate() * HungerHibernationRate);
        lf.DeltaEnergy(sleepRate);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        return null;
    }
}
