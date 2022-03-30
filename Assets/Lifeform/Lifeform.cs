using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifeform : MonoBehaviour
{
    // TODO: I feel like these could be bitwise flagged in an enum?
    //  Or, do we check the state machine state instead?
    [SerializeField]
    private bool m_Moving;
    [SerializeField]
    private bool m_Eating;
    [SerializeField]
    private bool m_Sleeping;

    [SerializeField]
    private float m_Hunger;
    [SerializeField]
    private float m_Age;
    [SerializeField]
    private float m_Energy;

    [SerializeField]
    private LifeformNavigation m_Navigation;

    [SerializeField]
    private LifeformGenetics m_Genetics;
    
    private float m_Eyesight;
    private float m_MaxHunger;
    private float m_MaxAge;
    private float m_MaxEnergy;
    private float m_AgeRate;
    private float m_HungerRate;
    private float m_MoveRate;
    private float m_EnergyRate;
    private float m_SleepRate;

    public bool Moving => m_Moving; 
    public bool Eating => m_Eating;
    public bool Sleeping => m_Sleeping;
    public float Hunger => m_Hunger;
    public float Age => m_Age;
    public float Energy => m_Energy;

    public LifeformNavigation GetNavigation() => m_Navigation;

    private float GetAgeRate() 
    {
        return Time.deltaTime * m_AgeRate;
    }

    private float GetSleepRate()
    {
        return Time.deltaTime * m_SleepRate;
    }
    
    public float GetEnergyRate() 
    {
        return Time.deltaTime * m_EnergyRate;
    }
    
    public float GetEyesightDistance() 
    {
        return m_Eyesight;
    }
    
    public float GetMoveRate()
    {
        return m_MoveRate;
    }

    public float GetMaxEnergy()
    {
        return m_MaxEnergy;
    }
    
    public float GetHungerRate()
    {
        return Time.deltaTime * m_HungerRate;
    }

    public float GetMaxHunger()
    {
        return m_MaxHunger;
    }
    
    public void Eat(float increment) 
    {
        Stop();
        m_Eating = true;
        m_Hunger += increment;
    }

    public void Sleep() 
    {
        Stop();
        m_Sleeping = true;
        m_Energy += GetSleepRate();
    }

    public void Move()
    {
        m_Moving = true;
        m_Eating = false;
        m_Sleeping = false;
    }

    public void Stop()
    {
        m_Moving = false;
        m_Eating = false;
        m_Sleeping = false;
    }

    public void IncrementAge()
    {
        m_Age += GetAgeRate();
    }

    public void DecrementHunger()
    {
        m_Hunger -= GetHungerRate();
    }

    public void DecrementEnergy()
    {
        m_Energy -= GetEnergyRate();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Populate from genetics
        m_Eyesight = m_Genetics.GetEyesight();
        m_MaxHunger = m_Genetics.GetMaxHunger();
        m_MaxAge = m_Genetics.GetMaxAge();
        m_MaxEnergy = m_Genetics.GetMaxEnergy();
        m_AgeRate = m_Genetics.GetMaxAge();
        m_HungerRate = m_Genetics.GetHungerRate();
        m_MoveRate = m_Genetics.GetMoveRate();
        m_EnergyRate = m_Genetics.GetEnergyRate();
        m_SleepRate = m_Genetics.GetSleepRate();

        // Set to maximums
        m_Hunger = m_MaxHunger;
        m_Energy = m_MaxEnergy;
    } 
}
