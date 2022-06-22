using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

// A behaviour which spawns and simulates lifeforms
public class Incubator: MonoBehaviour
{
    public delegate Lifeform RandomLifeFactoryDelegate();
    public delegate Lifeform EvolutionFactoryDelegate(Lifeform parent1, Lifeform parent2);
    
    private List<Lifeform> m_Pool;

    // Number of total lifeforms to simulate and manage.
    [SerializeField]
    private int m_PoolSize = 256;

    // Number of lifeforms to select and breed when m_BreedFittest is enabled
    [SerializeField]
    private int m_PoolBatchSize = 16;

    [SerializeField]
    private float m_SpawnRadius = 5f;

    // The lifeform prefab to use when spawning
    [SerializeField]
    private GameObject m_LifeformPrefab;

    // Enable to automatically breed the fittest Lifeform's together
    [SerializeField]
    private bool m_BreedFittest = true;

    private RandomLifeFactoryDelegate m_RandomLifeFactory;
    private EvolutionFactoryDelegate m_EvolutionFactory;

    // Since we are mutating bytes when breeding, these numbers go out of bounds.
    // Not sure how best to manage this, or have it just die...
    private static bool HasMinimumViableGenetics(Lifeform lf)
    {
        LifeformGenetics g = lf.Genetics;

        float rHunger = g.GetHungerRate();
        float rSleep = g.GetSleepRate();
        float rEnergy = g.GetEnergyRate();
        float eyesight = g.GetEyesightDistance();
        float rMove = g.GetMoveRate();

        return rHunger > 0.00001f
            && rSleep > 0.00001f
            && rEnergy > 0.000001f
            && eyesight > 0.5f
            && rMove > 0;
    }

    private void DisposeLifeform(Lifeform lf)
    {
        if(lf == null)
          return;

        GameObject.Destroy(lf.gameObject);
    }

    private Lifeform DefaultRandomLifeFactory() 
    {
        GameObject lifeformObj = Instantiate(m_LifeformPrefab, transform.parent);
        lifeformObj.name = string.Format("Lifeform{0}", lifeformObj.GetHashCode());
        
        Vector3 offset = transform.position;
        Vector3 randomAround = Random.insideUnitCircle * m_SpawnRadius;
        lifeformObj.transform.position = transform.position 
          + new Vector3(randomAround.x, 2f, randomAround.y);

        if(lifeformObj.GetComponent<LifeformStateMachine>() == null)
          throw new Exception();

        Lifeform l = lifeformObj.GetComponent<Lifeform>();
        LifeformGenetics lg = lifeformObj.AddComponent<LifeformGenetics>();
        l.Initialize(lg);
        
        return l;
    }

    private Lifeform DefaultEvolutionFactory(Lifeform parent1, Lifeform parent2) 
    {
        return parent1.Breed(parent2);
    }

    void Start()
    {
        m_Pool = new List<Lifeform>(m_PoolSize);
        m_RandomLifeFactory = DefaultRandomLifeFactory;
        m_EvolutionFactory = DefaultEvolutionFactory;
    }
    
    private void FillPool()
    {
        int generate = m_PoolSize - m_Pool.Count();
        while(generate > 0)
        {
            Lifeform newLife = m_RandomLifeFactory();
            if(!HasMinimumViableGenetics(newLife)) 
            {
                DisposeLifeform(newLife);
                continue;
            }

            m_Pool.Add(newLife);
            generate--;
        }
    }

    // TODO: dirty, coffee inspired
    private List<Lifeform> BreedFittest(Lifeform fittest)
    {
        var offspring = new List<Lifeform>();

        Lifeform[] longestLivingLifeforms = m_Pool
          .OrderByDescending(p => p.GetAliveTime())
          .Take(m_PoolBatchSize)
          .ToArray();

        for(int y = 0; y < longestLivingLifeforms.Length; y++)
        {
            Lifeform candidate = longestLivingLifeforms[y];
            if(candidate == null) 
                continue;

            // Avoid hermaphrodites for now...
            if(fittest == candidate)
                continue;
            
            bool valid = false;
            while(!valid)
            {
                if(fittest == null || candidate == null)
                  break;

                Lifeform child = m_EvolutionFactory.Invoke(fittest, candidate);
                if(HasMinimumViableGenetics(child)) 
                {
                    child.gameObject.transform.position = fittest.transform.position 
                      + Vector3.up * 2.0f;

                    valid = true;
                    offspring.Add(child);
                    break;
                }
                else 
                    DisposeLifeform(child);
            } 
        }

        return offspring;
    }

    // TODO: not performant in the slightest
    //  Current fitness criteria is a simple rank of oldest surviving genome.
    public void Incubate() 
    {
        Lifeform fittest = null;
        float longestLiving = 0;

        for(int x = 0; x < m_Pool.Count(); x++)
        {
            Lifeform l1 = m_Pool[x];
            if(l1 == null) 
            {
                m_Pool.RemoveAt(x);
                continue;
            }

            if(!HasMinimumViableGenetics(l1))
            {
                m_Pool.RemoveAt(x);
                DisposeLifeform(l1);
                continue;
            }

            float l1time = l1.GetAliveTime();
            if(l1time > longestLiving)
            {
                fittest = l1;
                longestLiving = l1time;
                continue;
            }
        }

        if(fittest == null || !m_BreedFittest)
          return;
        
        List<Lifeform> offspring = BreedFittest(fittest); 
        m_Pool.AddRange(offspring);
    }

    void Update()
    {
        // Re populate the pool once completely depleted
        if(m_Pool.Count() == 0) 
        {
            FillPool();
            return;
        }

        Incubate();
    }
}
