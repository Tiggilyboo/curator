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
    private float m_HungerRate;
    [SerializeField]
    private float m_MoveRate;
    [SerializeField]
    private float m_EnergyRate;
    [SerializeField]
    private float m_SleepRate;
    [SerializeField]
    private float m_BreedRate;

    // Currently just consume the start byte and use u8 as the upperbound
    private float TraitToByteFloat(Trait t)
    {
        byte gene = GetTrait(t).AsByte(this);

        return (float)gene / 255.0f;
    }

    // [-1, 1]
    private float TraitToByteUnitFloat(Trait t)
    {
        return 1.0f - TraitToByteFloat(t) * 2.0f;
    }

    // TODO: Too magical
    public override void Initialize()
    {
        base.Initialize();

        m_MaxAge = Mathf.Abs(GetTrait(Trait.Age).AsInt(this)) % (2 * 60 * 60);
        m_MaxEnergy = TraitToByteFloat(Trait.Energy) * 500;
        m_MaxHunger = TraitToByteFloat(Trait.Hunger) * 100;
        m_MoveRate = TraitToByteFloat(Trait.Speed) * 10;
        m_Eyesight = TraitToByteFloat(Trait.Eyesight) * 25;
        m_SleepRate = TraitToByteUnitFloat(Trait.Energy);
        m_HungerRate = TraitToByteFloat(Trait.Hunger);
        m_EnergyRate = TraitToByteFloat(Trait.Energy);
        m_BreedRate = TraitToByteFloat(Trait.Breed);
    }
    
    public float GetEyesightDistance() => m_Eyesight;
    public float GetMaxEnergy() => m_MaxEnergy;
    public float GetMaxHunger() => m_MaxHunger;
    public float GetMaxAge() => m_MaxAge;

    public float GetEnergyRate() => Time.deltaTime * m_EnergyRate;
    public float GetHungerRate() => Time.deltaTime * m_HungerRate;
    public float GetSleepRate() => Time.deltaTime * m_SleepRate;
    public float GetMoveRate() => m_MoveRate;
    public float GetBreedRate() => m_BreedRate;
}
