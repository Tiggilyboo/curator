using System;
using UnityEngine;
using Trait = GeneticTraitType;

public class LifeformGenetics: Genetics
{
    [SerializeField]
    private float m_Eyesight;
    [SerializeField]
    private float m_MaxHunger;
    [SerializeField]
    private float m_MaxAge;
    [SerializeField]
    private float m_MaxEnergy;
    [SerializeField]
    private float m_AgeRate;
    [SerializeField]
    private float m_HungerRate;
    [SerializeField]
    private float m_MoveRate;
    [SerializeField]
    private float m_EnergyRate;
    [SerializeField]
    private float m_SleepRate;

    public override void Initialize()
    {
        base.Initialize();

        m_MaxEnergy = GetTrait(Trait.Energy).AsFloat(this);
        m_MaxHunger = GetTrait(Trait.Hunger).AsFloat(this);
        m_MaxAge = GetTrait(Trait.Age).AsInt(this);
        m_MoveRate = GetTrait(Trait.Speed).AsFloat(this);
        m_Eyesight = GetTrait(Trait.Eyesight).AsFloat(this);
        m_SleepRate = GetTrait(Trait.Energy).AsUnitFloat(this);
        m_HungerRate = GetTrait(Trait.Hunger).AsUnitFloat(this);
        m_EnergyRate = GetTrait(Trait.Energy).AsUnitFloat(this);

        m_AgeRate = m_MaxAge % 1.0f;
    }

    public float GetAgeRate() => Time.deltaTime * m_AgeRate;
    public float GetEnergyRate() => Time.deltaTime * m_EnergyRate;
    public float GetHungerRate() => Time.deltaTime * m_HungerRate;
    public float GetSleepRate() => Time.deltaTime * m_SleepRate;

    public float GetMoveRate() => m_MoveRate;
    public float GetEyesightDistance() => m_Eyesight;
    public float GetMaxEnergy() => m_MaxEnergy;
    public float GetMaxHunger() => m_MaxHunger;
    public float GetMaxAge() => m_MaxAge;
}
