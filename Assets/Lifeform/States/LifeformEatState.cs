using UnityEngine;

public class LifeformEatState: IState<Lifeform>
{
    private static LifeformEatState m_Instance = new LifeformEatState();
    public static LifeformEatState Instance => m_Instance;

    public string Identifier => "Eating";

    private const float ReactToEnergyThreshold = 0.05f;

    public void OnExit(Lifeform lf){}
    public void OnEntry(Lifeform lf){}

    private bool AttemptToEat(Lifeform lf)
    {
        return lf.Inventory.TryToEat();
    }

    public IState<Lifeform> UpdateState(Lifeform lf)
    { 
        float energyRate = -lf.Genetics.GetEnergyRate();
        if(lf.Energy + energyRate <= lf.Genetics.GetMaxEnergy() * ReactToEnergyThreshold)
          return LifeformIdleState.Instance;

        lf.Navigation.ResetPath();
        lf.Navigation.Stop();

        bool eaten = AttemptToEat(lf);
        if(lf.Hunger >= lf.Genetics.GetMaxHunger())
          return LifeformIdleState.Instance;
        
        if(!eaten)
          lf.DeltaHunger(-lf.Genetics.GetHungerRate());

        lf.DeltaAge();
        lf.DeltaEnergy(energyRate);

        if(lf.IsDying())
          return LifeformDeadState.Instance;

        // Find food
        if(!eaten)
            return LifeformWanderState.Instance;

        return null;
    }
}
