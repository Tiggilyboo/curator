using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class LifeformStateExtensions
{
    public static void Delta(this Lifeform lf)
    {
        lf.DeltaAge();
        lf.DeltaEnergy(-lf.Genetics.GetEnergyRate());
        lf.DeltaHunger(-lf.Genetics.GetHungerRate());
    }

    public static void DeltaMove(this Lifeform lf)
    {
        lf.DeltaAge();
        lf.DeltaHunger(-lf.Genetics.GetHungerRate());

        // Further incur penalty for velocity
        Vector3 velocity = lf.Navigation.GetVelocity();
        float moveRate = lf.Genetics.GetMoveRate();
        lf.DeltaEnergy(-moveRate * velocity.sqrMagnitude * 0.001f);
    } 

    public static bool IsDying(this Lifeform lf)
    {
        return lf.Energy <= 0
          || lf.Hunger <= 0
          || lf.Age >= lf.Genetics.GetMaxAge();
    }
    
    public static bool SleepConditions(this Lifeform lf)
    {
        return lf.Energy <= lf.Genetics.GetMaxEnergy() * 0.25f;
    }

    public static bool EatConditions(this Lifeform lf)
    {
        return lf.Hunger <= lf.Genetics.GetMaxHunger() * 0.25f;
    }

    public static bool BreedConditions(this Lifeform lf)
    {
        return lf.Age >= lf.Genetics.GetMaxAge() * 0.01f;
    }
    
    public static bool WanderConditions(this Lifeform lf)
    {
        if(!lf.Navigation.HasPath() || lf.Interests.Any())
          return true;

        return false;
    }
}
