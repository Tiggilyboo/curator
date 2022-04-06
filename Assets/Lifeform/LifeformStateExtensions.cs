using UnityEngine;

public static class LifeformStateExtensions
{
    public static void Delta(this Lifeform lf)
    {
        lf.DeltaAge();
        lf.DeltaEnergy(-lf.Genetics.GetEnergyRate());
        lf.DeltaHunger(-lf.Genetics.GetHungerRate());
    }

    public static bool IsDying(this Lifeform lf)
    {
        return lf.Energy <= 0
          || lf.Hunger <= 0
          || lf.Age >= lf.Genetics.GetMaxAge();
    }
}
