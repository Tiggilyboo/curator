using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incubator: MonoBehaviour
{
    public delegate Lifeform RandomLifeFactoryDelegate();
    public delegate Lifeform EvolutionFactoryDelegate(Lifeform parent1, Lifeform parent2);
    
    private List<Lifeform> m_Pool;

    [SerializeField]
    private int m_PoolSize = 64;
    [SerializeField]
    private int m_PoolBatchSize = 4;
    [SerializeField]
    private GameObject m_LifeformPrefab;
    [SerializeField]
    private bool m_LiveSimulation = true;

    private RandomLifeFactoryDelegate m_RandomLifeFactory;
    private EvolutionFactoryDelegate m_EvolutionFactory;

    private Lifeform DefaultRandomLifeFactory() 
    {
        GameObject lifeformObj = Instantiate(m_LifeformPrefab, transform.parent);
        lifeformObj.name = "Lifeform";

        lifeformObj.SetActive(m_LiveSimulation);

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
           m_Pool.Add(m_RandomLifeFactory.Invoke()); 
           generate--;

           Debug.Log(string.Format("Generated random life: {0} remaining", generate));
        }
    }

    public void Incubate() 
    {
        if(m_Pool.Count(l => l.Dead) < m_PoolBatchSize)
          return;

        List<Lifeform> aliveTheLongest = m_Pool
          .Where(p => p.Dead)
          .OrderByDescending(p => p.GetAliveTime())
          .ToList();

        // TODO: not performant in the slightest
        List<Lifeform> offspring = new List<Lifeform>();
        for(int x = 0; x < aliveTheLongest.Count(); x++)
        {
            Lifeform l1 = aliveTheLongest[x];
            if(l1 == null)
              aliveTheLongest.RemoveAt(x);

            for(int y = 0; y < aliveTheLongest.Count(); y++)
            {
                if(x >= m_PoolBatchSize || y >= m_PoolBatchSize)
                  continue;

                Lifeform l2 = aliveTheLongest[y];
                if(l2 == null)
                  aliveTheLongest.RemoveAt(y);

                // Super insestual anyways, but hermaphroditic is just too far...
                if(l1 == l2)
                  continue;

                Lifeform child = m_EvolutionFactory.Invoke(l1, l2);
                offspring.Add(child);
            }

            if(x >= m_PoolBatchSize) 
            {
                GameObject.Destroy(l1.gameObject);
                aliveTheLongest.RemoveAt(x);
                continue;
            }

            l1.Reset(); 
        }
        
        m_Pool.Clear();
        m_Pool.AddRange(aliveTheLongest);
        m_Pool.AddRange(offspring);
    }

    void Update()
    {
        if(m_Pool.Count() == 0) {
            FillPool();
            return;
        }

        Incubate();
    }
}
