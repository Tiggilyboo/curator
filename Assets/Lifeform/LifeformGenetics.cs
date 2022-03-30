using UnityEngine;
using Trait = GeneticTraitType;

public class LifeformGenetics: MonoBehaviour
{
    [SerializeField]
    private Genetics m_Genetics;
    
    public float GetMaxEnergy() => m_Genetics.GetTrait(Trait.Energy).AsFloat(m_Genetics);
    public float GetMaxHunger() => m_Genetics.GetTrait(Trait.Hunger).AsFloat(m_Genetics);
    public int GetMaxAge() => m_Genetics.GetTrait(Trait.Age).AsInt(m_Genetics);
    public float GetMoveRate() => m_Genetics.GetTrait(Trait.Speed).AsFloat(m_Genetics);
    public float GetEyesight() => m_Genetics.GetTrait(Trait.Eyesight).AsFloat(m_Genetics);

    public float GetSleepRate() => m_Genetics.GetTrait(Trait.Energy).AsUnitFloat(m_Genetics);
    public float GetHungerRate() => m_Genetics.GetTrait(Trait.Hunger).AsUnitFloat(m_Genetics);
    public float GetEnergyRate() => m_Genetics.GetTrait(Trait.Energy).AsUnitFloat(m_Genetics);

    private bool DeadOnArrival() {
        return GetMaxEnergy() < 10.0f
          || GetMaxHunger() <= 1.0f
          || GetMaxAge() < 10
          || GetMoveRate() < 1.0f
          || GetEyesight() < 1.0f
          || GetSleepRate() <= 0.0f
          || GetHungerRate() <= 0.0f
          || GetEnergyRate() <= 0.0f;
          
    }

    void Start()
    {
        if(m_Genetics == null) 
        {
            m_Genetics = gameObject.AddComponent<Genetics>() as Genetics;

            do {
              m_Genetics.InitializeGenetics();
            }
            while(DeadOnArrival());
        }
    }
}
