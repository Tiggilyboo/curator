using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This is a horrific mess. Split some of these things up
public class Lifeform : MonoBehaviour
{
    private bool m_Initialized;
    // In order to judge fitness, we guage how long they have stayed alive...
    private float m_TimeOfBirth;
    private float m_TimeOfDeath;

    // TODO: I feel like these could be bitwise flagged in an enum?
    //  Or, do we check the state machine state instead?
    [SerializeField]
    private bool m_Moving;
    [SerializeField]
    private bool m_Eating;
    [SerializeField]
    private bool m_Sleeping;
    [SerializeField]
    private bool m_Dead;

    [SerializeField]
    private float m_Hunger;
    [SerializeField]
    private float m_Age;
    [SerializeField]
    private float m_Energy;

    [SerializeField]
    private LifeformGenetics m_Genetics;
    [SerializeField]
    private LifeformNavigation m_Navigation;
    [SerializeField]
    private LifeformStateMachine m_StateMachine;

    [SerializeField]
    private GameObject m_LifeformPrefab;

    // Lifeform Stats
    public bool Moving => m_Moving; 
    public bool Eating => m_Eating;
    public bool Sleeping => m_Sleeping;
    public bool Dead => m_Dead;
    public float Hunger => m_Hunger;
    public float Age => m_Age;
    public float Energy => m_Energy;

    public LifeformNavigation Navigation => m_Navigation;
    public LifeformGenetics Genetics => m_Genetics;
    public LifeformStateMachine StateMachine => m_StateMachine;

    public float GetBirthTime() => m_TimeOfBirth;
    public float GetAliveTime() => m_TimeOfDeath - m_TimeOfBirth;

    public void Die()
    {
        Stop();
        m_Dead = true;
        m_TimeOfDeath = Time.realtimeSinceStartup;
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
        m_Energy += Genetics.GetSleepRate();
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

    public Lifeform Breed(Lifeform other)
    {
        GameObject childObj = Instantiate(m_LifeformPrefab, transform.parent);
        childObj.name = "Lifeform";
        Lifeform child = childObj.GetComponent<Lifeform>();
        LifeformGenetics childGenes = child.Genetics;
        child.Initialize(childGenes);

        return child;
    }

    public void IncrementAge()
    {
        m_Age += Genetics.GetAgeRate();
    }

    public void DecrementHunger()
    {
        m_Hunger -= Genetics.GetHungerRate();
    }

    public void DecrementEnergy()
    {
        m_Energy -= Genetics.GetEnergyRate();
    }

    public void Reset()
    {
        m_Initialized = false;
        Initialize(m_Genetics);
    }

    // TODO: Surely theres a better way... with this madness of hierarchy & codependence
    public void Initialize(LifeformGenetics genetics)
    {
        if(m_Initialized)
          return;

        // Ensure genetics are initialized before starting
        if(genetics == null)
          throw new NullReferenceException("Unable to initialize without genetics being set");

        m_Genetics = genetics;
        m_Genetics.Initialize();

        float moveRate = m_Genetics.GetMoveRate();
        if(moveRate != float.NaN && moveRate > 0.0)
        {
            m_Navigation.SetMoveRate(m_Genetics.GetMoveRate());

            // Set to maximums / start values
            m_Hunger = m_Genetics.GetMaxHunger();
            m_Energy = m_Genetics.GetMaxEnergy();
            m_Age = 0;

            m_TimeOfBirth = Time.realtimeSinceStartup;
            m_TimeOfDeath = 0;
            m_Initialized = true;
        } 
        else  // Probably not the greatest, but saves a few iterations 
        {
            Die();
        }
    } 
}
