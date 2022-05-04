using UnityEngine;

public class LifeformEatState: IState<Lifeform>
{
    private static LifeformEatState m_Instance = new LifeformEatState();
    public static LifeformEatState Instance => m_Instance;

    public string Identifier => "Eating";

    private const float ReactToEnergyThreshold = 0.05f;

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

        float energyRate = -lf.Genetics.GetEnergyRate();
        if(lf.Energy + energyRate <= lf.Genetics.GetMaxEnergy() * ReactToEnergyThreshold)
          return LifeformIdleState.Instance;
        
        lf.Navigation.Stop();
        lf.DeltaAge();
        lf.DeltaEnergy(energyRate);
        lf.DeltaHunger(hungerRate);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        return null;
    }
}
