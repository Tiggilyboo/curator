using System;
using UnityEngine;
using Trait = GeneticTraitType;

public class LifeformGenetics: MonoBehaviour
{
    private bool m_Initialized;

    [SerializeField]
    private Genetics m_Genetics;

    [SerializeField]
    private float m_MaxHunger;
    [SerializeField]
    private float m_MaxAge;
    [SerializeField]
    private float m_MaxEnergy;
    [SerializeField]
    private float m_HungerRate;
    [SerializeField]
    private float m_MoveRate;
    [SerializeField]
    private float m_EnergyRate;
    [SerializeField]
    private float m_SleepRate;
    [SerializeField]
    private float m_BreedRate;
    [SerializeField]
    private float m_Eyesight;
    
    public bool Initialized => m_Initialized;
    
    public float GetMaxEnergy() => m_MaxEnergy;
    public float GetMaxHunger() => m_MaxHunger;
    public float GetMaxAge() => m_MaxAge;

    public float GetEnergyRate() => Time.deltaTime * m_EnergyRate;
    public float GetHungerRate() => Time.deltaTime * m_HungerRate;
    public float GetSleepRate() => Time.deltaTime * m_SleepRate;
    public float GetMoveRate() => m_MoveRate;
    public float GetBreedRate() => m_BreedRate;
    public float GetEyesightDistance() => m_Eyesight;
    public Genetics GetGenetics() => m_Genetics;

    private GeneticTrait GetTrait(Trait t) => m_Genetics.GetTrait(t);

    // Currently just consume the start byte and use u8 as the upperbound
    private float TraitToByteFloat(Trait t)
    {
        byte gene = GetTrait(t).AsByte(m_Genetics);

        return (float)gene / 255.0f;
    }


    // TODO: Too magical
    private void InitializeFields()
    {
        m_MaxAge = Mathf.Abs(GetTrait(Trait.Age).AsInt(m_Genetics)) % (2 * 60 * 60);
        m_MaxEnergy = TraitToByteFloat(Trait.Energy) * 500;
        m_MaxHunger = TraitToByteFloat(Trait.Hunger) * 100;
        m_MoveRate = TraitToByteFloat(Trait.Speed) * 10;
        m_Eyesight = TraitToByteFloat(Trait.Eyesight) * 25;
        m_SleepRate = TraitToByteFloat(Trait.Energy);
        m_HungerRate = TraitToByteFloat(Trait.Hunger);
        m_EnergyRate = TraitToByteFloat(Trait.Energy);
        m_BreedRate = TraitToByteFloat(Trait.Breed);
        m_Initialized = true;
    }

    public void Initialize()
    {
        if(m_Initialized)
          return;

        m_Genetics = new Genetics();
        InitializeFields();
    }

    public void InitializeFromParents(LifeformGenetics parent1, LifeformGenetics parent2)
    {
        if(m_Initialized)
          return;

        m_Genetics = new Genetics(
            (Genetics)parent1.GetGenetics().Clone(), 
            (Genetics)parent2.GetGenetics().Clone());

        InitializeFields();
    }
}
