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
    private float m_MaxHunger = 50.0f;
    private float m_MaxAge = 100.0f;
    private float m_MaxEnergy = 30.0f;
    private float m_AgeRate = 0.0001f;
    private float m_HungerRate = 0.001f;
    private float m_MoveRate = 5.0f;

    public bool Moving => m_Moving; 
    public bool Eating => m_Eating;
    public bool Sleeping => m_Sleeping;
    public float Hunger => m_Hunger;
    public float Age => m_Age;
    public float Energy => m_Energy;

    private float GetMoveRate() 
    {
        float movingRate = m_MoveRate;
        if (Moving)
        {
            movingRate *= 3.0f;
        }

        return Time.deltaTime * movingRate;
    }

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
        return Time.deltaTime;
    }
    
    public float GetEyesightDistance() 
    {
        return m_Eyesight;
    }
    
    public void Eat(float increment) 
    {
        m_Eating = true;
        m_Hunger += increment;
    }

    public void Sleep() 
    {
        m_Sleeping = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        m_Hunger = m_MaxHunger;
        m_Energy = m_MaxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Sleeping) 
        {
            m_Energy += GetSleepRate(); 
            if(m_Energy > m_MaxEnergy) 
            {
              m_Energy = m_MaxEnergy;
              m_Sleeping = false;
            }
        } 
        else 
        {
            m_Energy -= GetMoveRate();
        }

        if(!m_Eating)
        {
            m_Hunger -= GetHungerRate();
        }

        m_Age += GetAgeRate();
    }
}
