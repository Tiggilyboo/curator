using System;
using System.Collections.Generic;
using UnityEngine;
using Trait = GeneticTraitType;

public class LifeformGenetics: MonoBehaviour
{
    private bool m_Initialized;
    public bool Initialized => m_Initialized;

    [SerializeField]
    private Genetics m_Genetics;
    [SerializeField]
    private GameObject m_LifeformBody;

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
    
    public float GetMaxEnergy() => m_MaxEnergy;
    public float GetMaxHunger() => m_MaxHunger;
    public float GetMaxAge() => m_MaxAge;

    public float GetEnergyRate() => m_EnergyRate;
    public float GetHungerRate() => m_HungerRate;
    public float GetSleepRate() => m_SleepRate;
    public float GetMoveRate() => m_MoveRate;
    public float GetBreedRate() => m_BreedRate;
    public float GetEyesightDistance() => m_Eyesight;
    private Genetics CloneUnderlying() => (Genetics)m_Genetics.Clone();
    public IEnumerable<byte> GetData() => m_Genetics.GetData();
    public IEnumerable<byte> GetDataForTrait(GeneticTrait trait) => m_Genetics.GetDataForTrait(trait);
    public int GetDataSize() => m_Genetics.GetDataSize();
    public int GetTraitCount() => m_Genetics.GetTraitCount();
    public GeneticTrait GetTrait(Trait t) => m_Genetics.GetTrait(t);

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
        m_MoveRate = TraitToByteFloat(Trait.Speed);
        m_Eyesight = TraitToByteFloat(Trait.Eyesight) * 25;
        m_SleepRate = TraitToByteFloat(Trait.Energy);
        m_HungerRate = TraitToByteFloat(Trait.Energy); // 1f? Maybe need other factors
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
            (Genetics)parent1.CloneUnderlying(), 
            (Genetics)parent2.CloneUnderlying());

        InitializeFields();
    }
}
