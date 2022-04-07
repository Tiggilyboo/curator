using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Genetics: ICloneable
{
    private byte[] m_Data;
    
    [SerializeField]
    private GeneticTrait[] m_Traits;
    [SerializeField]
    private Genetics[] m_Parents;

    [SerializeField]
    private int m_GeneticDataSize = 32;
    [SerializeField]
    private float m_CrossoverRate = 0.65f;
    [SerializeField]
    private float m_MutationRate = 0.01f;
    
    private static GeneticTraitType GetTraitType(int idx) 
    {
        return (GeneticTraitType)(idx % (int)GeneticTraitType.COUNT);
    }
    
    public IEnumerable<byte> GetData() => m_Data;
    public int GetDataSize() => m_Data.Length;
    public int GetTraitCount() => m_Traits.Length;
    public float GetCrossoverRate() => m_CrossoverRate;
    public float GetMutationRate() => m_MutationRate;
    public Genetics[] GetParents() => m_Parents;

    // Determine a crossover value between two values based on the genetics crossover ratio
    private int CrossValue(int parent1Size, int parent2Size) 
    {
        float crossChance = Random.value * m_CrossoverRate;
        int minSize = Mathf.Min(parent1Size, parent2Size);
        int maxSize = Mathf.Max(parent1Size, parent2Size);
        int newSize = (int)(minSize + ((maxSize - minSize) * crossChance));

        return newSize;
    }
    
    // Determine a child crossover rate between two parents
    private float CrossRate(float p1Rate, float p2Rate) 
    {
        float min = Mathf.Min(p1Rate, p2Rate);
        float max = Mathf.Max(p1Rate, p2Rate);

        return (min + ((max - min) * Random.value));
    }
    
    // Copy part of another genetic makeup into this one split by a position / index
    private void SwapWith(Genetics otherGenes, int position) 
    {
        for(int i = 0; i < position; i++) 
        {
            m_Data[i] = otherGenes.GetData().ElementAt(i);
        }

        // Swap traits contained within the 0..position
        for(int i = 0; i < m_Traits.Length; i++) 
        {
            GeneticTrait swapTrait = otherGenes.GetTrait((GeneticTraitType)i);
            int start = swapTrait.Start;
            int end;
            
            // It fits, copy
            if(swapTrait.Start < position && swapTrait.End < position) 
            {
                end = swapTrait.End;
            } 
            // If it sorta fits, jam it.
            else if(swapTrait.Start < position && swapTrait.End >= position) 
            {
                end = position;
            }
            // It certainly does not fit. The trait is still a member of the current genetics.
            else 
              continue;

            m_Traits[i] = new GeneticTrait(swapTrait.Identifier, start, end);
        }
    }
    
    // Create a new randomly generated Genetics instance
    public Genetics()
    {
        const int traitCount = (int)GeneticTraitType.COUNT;

        m_Data = new byte[m_GeneticDataSize];
        m_Traits = new GeneticTrait[traitCount];

        // Uniformly allocate each trait by the available data size
        int traitSpans = Mathf.FloorToInt(m_GeneticDataSize / traitCount);
        for(int i = 0; i < traitCount; i++) 
        {
            int start = traitSpans * i;
            int end = start + traitSpans;
            GeneticTraitType traitType = GetTraitType(i);

            m_Traits[i] = new GeneticTrait(traitType, start, end);
        }

        // TODO: Could bitwise op this from in byte chunks if looking for perf improvements
        for(int i = 0; i < m_GeneticDataSize; i++)
          m_Data[i] = (byte)Random.Range(0, 255);
    }
    
    // Create a new genetics instance from two parents
    public Genetics(Genetics parent1, Genetics parent2) 
    {
        m_CrossoverRate = CrossRate(parent1.GetCrossoverRate(), parent2.GetCrossoverRate());
        int newSize = CrossValue(parent1.GetDataSize(), parent2.GetDataSize());
        int newTSize = CrossValue(parent1.GetTraitCount(), parent2.GetTraitCount());

        m_GeneticDataSize = newSize;
        m_Data = new byte[newSize];
        m_Traits = new GeneticTrait[newTSize];
        m_Parents = new Genetics[] { 
            parent1,
            parent2
        };

        // Crossover
        float crossAt = Random.value;
        if(m_CrossoverRate > 0 && crossAt <= m_CrossoverRate) 
        {
            int crossDSize = (int)(newSize * crossAt);
            int crossTSize = (int)(newTSize * crossAt);
            
            IEnumerable<byte> parentData = parent1.GetData();
            for(int i = 0; i < newSize; i++) 
            {
                m_Data[i] = parentData.ElementAt(i);
            }
            for(int i = crossTSize; i < newTSize; i++) 
            {
                GeneticTrait crossTrait = parent1.GetTrait((GeneticTraitType)i);
                m_Traits[i] = (GeneticTrait)crossTrait.Clone();
            }

            SwapWith(parent2, crossDSize);
        }
        // Copy from parent1
        else 
        {
            m_Data = parent1.GetData().ToArray();
            for(int i = 0; i < newTSize; i++) 
            {
                GeneticTrait parentTrait = parent1.GetTrait((GeneticTraitType)i);
                m_Traits[i] = (GeneticTrait)parentTrait.Clone();
            }
        }

        // Mutate
        for(int i = 0; i < m_Data.Length; i++) 
        {
            if(Random.value <= m_MutationRate)
              m_Data[i] = (byte)Random.Range(0, 255);
        }

        // Mutation to traits, ensure we do not overlap with the other traits
        int lastTraitBoundary = m_Traits.Length > 0 
          ? m_Traits[0].Start
          : 0;

        int maxBoundary = m_Data.Length / m_Traits.Length;
        for(int i = 0; i < m_Traits.Length; i++) 
        {
            // Mutate?
            if(Random.value < m_MutationRate) 
              continue;

            int start = Random.Range(lastTraitBoundary, maxBoundary);
            int end = Random.Range(start + 1, start + maxBoundary - 1);
            GeneticTraitType traitType = GetTraitType(i);

            m_Traits[i] = new GeneticTrait(traitType, start, end);
            lastTraitBoundary = end;
        }
    }

    public GeneticTrait GetTrait(GeneticTraitType tt) 
    {
        int idx = (int)tt;
        if(idx == (int)GeneticTraitType.COUNT)
          throw new ArgumentException("Invalid trait index");
        if(idx >= m_Traits.Length)
          throw new ArgumentException(string.Format(
                "Not enough traits, genetics only contains {0} traits, wanted {1}", m_Traits.Length, idx));

        return m_Traits[(int)tt];
    }

    public IEnumerable<byte> GetDataForTrait(GeneticTrait t)
    {
        return m_Data
            .Skip(t.Start)
            .Take(t.Count());
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
