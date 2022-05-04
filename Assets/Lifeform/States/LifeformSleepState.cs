using UnityEngine;

public class LifeformSleepState: IState<Lifeform>
{
    private static LifeformSleepState m_Instance = new LifeformSleepState();
    public static LifeformSleepState Instance => m_Instance;

    public string Identifier => "Sleeping";

    private const float HungerHibernationRate = 0.25f;
    private const float ReactToStarvationThreshold = 0.10f;

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
        
        float hungerRate = -lf.Genetics.GetHungerRate() * HungerHibernationRate;
        if(lf.Hunger + hungerRate <= lf.Genetics.GetMaxHunger() * ReactToStarvationThreshold)
          return LifeformIdleState.Instance;

        lf.Navigation.Stop();
        lf.DeltaAge();
        lf.DeltaHunger(hungerRate);
        lf.DeltaEnergy(sleepRate);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        return null;
    }
}
