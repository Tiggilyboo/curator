using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Lifeform : MonoBehaviour, IHaveResources
{
    private bool m_Initialized;
    private float m_TimeOfBirth;

    [SerializeField]
    private GameObject m_LifeformPrefab;

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
    private LifeformPerception m_Perception;
    [SerializeField]
    private LifeformInterests m_Interests;
    [SerializeField]
    private LifeformUI m_UI;
    [SerializeField]
    private LifeformInventory m_Inventory;

    public float Hunger => m_Hunger;
    public float Age => m_Age;
    public float Energy => m_Energy;

    public LifeformNavigation Navigation => m_Navigation;
    public LifeformGenetics Genetics => m_Genetics;
    public LifeformStateMachine StateMachine => m_StateMachine;
    public LifeformPerception Perception => m_Perception;
    public LifeformInterests Interests => m_Interests;
    public LifeformInventory Inventory => m_Inventory;
    public LifeformUI UI => m_UI;

    public GameObject GetPrefab() => m_LifeformPrefab;
    public float GetBirthTime() => m_TimeOfBirth;
    public float GetAliveTime() => Time.realtimeSinceStartup - m_TimeOfBirth;
    public ResourceStorage GetResourceStorage() => m_Inventory.GetResourceStorage();

    public void DeltaAge()
    {
        m_Age += Time.deltaTime;
    }

    public void DeltaHunger(float rate)
    {
        m_Hunger += rate * Time.deltaTime;
    }

    public void DeltaEnergy(float rate)
    {
        m_Energy += rate * Time.deltaTime;
    }

    public void SetEnergy(float energy)
    {
        m_Energy = energy;
    }

    public Lifeform Breed(Lifeform other)
    {
        GameObject childObj = Instantiate(m_LifeformPrefab, transform.parent);
        childObj.name = string.Format("Lifeform{0}", childObj.GetHashCode());
        childObj.transform.position += Vector3.up;

        Lifeform child = childObj.GetComponent<Lifeform>();
        LifeformGenetics childGenes = child.Genetics;
        childGenes.InitializeFromParents(this.Genetics, other.Genetics);
        child.Initialize(childGenes);

        return child;
    }

    public void Initialize(LifeformGenetics genetics)
    {
        if (m_Initialized)
            return;

        // Ensure genetics are initialized before starting
        if (genetics == null)
            throw new NullReferenceException("Unable to initialize without genetics being set");

        m_Genetics = genetics;
        if (!m_Genetics.Initialized)
            m_Genetics.Initialize();

        float moveRate = m_Genetics.GetMoveRate();
        if (moveRate != float.NaN && moveRate > 0.0)
        {
            m_Navigation.SetMoveRate(m_Genetics.GetMoveRate());
        }

        // Set to maximums / start values
        m_Hunger = m_Genetics.GetMaxHunger();
        m_Energy = m_Genetics.GetMaxEnergy();
        m_Age = 0;

        m_TimeOfBirth = Time.realtimeSinceStartup;
        m_Initialized = true;
    }
}
