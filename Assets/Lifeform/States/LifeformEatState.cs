using UnityEngine;

public class LifeformEatState: IState<Lifeform>
{
    private static LifeformEatState m_Instance = new LifeformEatState();
    public static LifeformEatState Instance => m_Instance;

    public string Identifier => "Eating";

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf)
    {
        lf.Navigation.ResetPath();
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    { 
        float hungerRate = lf.Genetics.GetHungerRate();
        if(lf.Hunger + hungerRate >= lf.Genetics.GetMaxHunger())
          return LifeformIdleState.Instance;
        
        lf.Navigation.Stop();
        lf.DeltaAge();
        lf.DeltaEnergy(-lf.Genetics.GetEnergyRate());
        lf.DeltaHunger(hungerRate);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        return null;
    }
}
