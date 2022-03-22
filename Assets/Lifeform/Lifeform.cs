using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifeform : MonoBehaviour
{
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
    
    // To later be derived from genetics
    private float m_Eyesight = 5.0f;
    private float m_MaxHunger = 10.0f;
    private float m_MaxAge = 100.0f;
    private float m_MaxEnergy = 10.0f;
    private float m_AgeRate = 0.0001f;
    private float m_HungerRate = 0.2f;
    private float m_MoveRate = 200.0f;
    private float m_EnergyRate = 0.1f;
    private float m_SleepRate = 3.0f;

    public bool Moving => m_Moving; 
    public bool Eating => m_Eating;
    public bool Sleeping => m_Sleeping;
    public float Hunger => m_Hunger;
    public float Age => m_Age;
    public float Energy => m_Energy;

    private float GetAgeRate() 
    {
        return Time.deltaTime * m_AgeRate;
    }

    private float GetHungerRate()
    {
        return Time.deltaTime * m_HungerRate;
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
        return Time.deltaTime * m_MoveRate;
    }

    public float GetMaxEnergy()
    {
        return m_MaxEnergy;
    }
    
    public void Eat(float increment) 
    {
        m_Eating = true;
        m_Hunger += increment;
    }

    public void Sleep() 
    {
        m_Sleeping = true;
        m_Moving = false;
        m_Eating = false;
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

    public void LookAt(Vector3 point) => transform.LookAt(point);
    public void Translate(Vector3 movement) => transform.Translate(movement);
    public Vector3 GetPosition() => transform.position;
    public Vector3 GetForward() => transform.forward;

    // Start is called before the first frame update
    void Start()
    {
        m_Hunger = m_MaxHunger;
        m_Energy = m_MaxEnergy;
    } 
}
