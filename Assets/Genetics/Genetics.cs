using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum GeneticTraitType 
{
    Energy,
    Hunger,
    Age,
    Speed,
    Eyesight,

    COUNT,
}

// This is essentially filling the void that .NET > 5.0 || Core > 2.1 has with Span<T> & MemoryMarshal
[Serializable]
public class GeneticTrait
{
    [SerializeField]
    private GeneticTraitType m_Type;
    [SerializeField]
    private int m_Start;
    [SerializeField]
    private int m_End;

    private T Convert<T>(Func<byte[], int, T> converter, Genetics genes)
    {
        if (m_Start >= m_End)
            throw new InvalidOperationException("Start index > end index");
        
        int size = Marshal.SizeOf<T>();
        IEnumerable<byte> geneSegment = genes.GetDataForTrait(this);

        // Ensure that we have enough data to return the appropriate type.
        if (m_End - m_Start <= size)
            return (T)converter.Invoke(geneSegment.ToArray(), m_Start);

        byte[] segmentRemaining = geneSegment.ToArray();

        Debug.Log(string.Format("trait: {0} [{1}, {2}]", m_Type, m_Start, m_End));
        Debug.Log(string.Format("size: {0}, segment size: {1}", size, segmentRemaining.Length));

        // Otherwise, create empty bytes to operate upon.
        byte[] newSegment = new byte[size];
        for (int i = 0; i < size; i++)
            newSegment[i] = segmentRemaining[i];

        return converter.Invoke(newSegment.ToArray(), 0);
    }

    public float AsFloat(Genetics gene) 
    {
        return Convert(BitConverter.ToSingle, gene);
    }

    public float AsUnitFloat(Genetics gene)
    {
        return Mathf.Abs(AsFloat(gene) % 1.0f);
    }
    
    public double AsDouble(Genetics gene)
    {
        return Convert(BitConverter.ToDouble, gene);
    }

    public int AsInt(Genetics gene)
    {
        return Convert(BitConverter.ToInt32, gene);
    }
    
    public long AsLong(Genetics gene)
    {
        return Convert(BitConverter.ToInt64, gene);
    }

    public char AsChar(Genetics gene)
    {
        return Convert(BitConverter.ToChar, gene);
    }

    public string AsString(Genetics gene)
    {
        if (m_Start > m_End)
            return string.Empty;

        IEnumerable<byte> geneSegment = gene.GetDataForTrait(this);

        int segCount = m_End - m_Start;
        byte[] segment = new byte[segCount];
        for (int i = m_Start; i < segCount; i++)
        {
            segment[i] = geneSegment.ElementAt(i);
        }

        int len = segCount / sizeof(char);
        StringBuilder sb = new StringBuilder() {
          Capacity = len,
        };

        for (int i = 0; i < len; i++)
        {
            sb.Append(AsChar(gene));
        }

        return sb.ToString();
    }

    public GeneticTraitType Identifier => m_Type;

    public int Count() => m_End - m_Start;
    public int Start => m_Start;
    public int End => m_End;

    public GeneticTrait(GeneticTraitType type, int start, int end)
    {
        if(start >= end)
          throw new ArgumentException("end must be > start");

        if(type == GeneticTraitType.COUNT)
          throw new ArgumentException("trait must be < count");

        m_Type = type;
        m_Start = start;
        m_End = end;
    }
}

public class Genetics : MonoBehaviour
{
    private byte[] m_Data;

    [SerializeField]
    private float m_CrossoverRate = 0.65f;
    [SerializeField]
    private float m_MutationRate = 0.08f;
    [SerializeField]
    private int m_GeneticDataSize = 32;
    [SerializeField]
    private Genetics[] m_Parents;
    [SerializeField]
    private GeneticTrait[] m_Traits;
    
    private static GeneticTraitType GetTraitType(int idx) {
        return (GeneticTraitType)(idx % (int)GeneticTraitType.COUNT);
    }

    public void InitializeGenetics()
    {
        int traitCount = (int)GeneticTraitType.COUNT;

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
    
    public void SwapWith(Genetics otherGenes, int position) 
    {
        for(int i = 0; i < position; i++) {
          m_Data[i] = otherGenes.GetData().ElementAt(i);
        }

        // Swap traits contained within the 0..positionData
        for(int i = 0; i < m_Traits.Length; i++) {
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
            else 
                throw new InvalidOperationException("unable to set trait end index");

            m_Traits[i] = new GeneticTrait(swapTrait.Identifier, start, end);
        }
    }

    public int CrossValue(int parent1Size, int parent2Size) 
    {
        float crossChance = Random.value * m_CrossoverRate;
        int minSize = Mathf.Min(parent1Size, parent2Size);
        int maxSize = Mathf.Max(parent1Size, parent2Size);
        int newSize = (int)(minSize + ((maxSize - minSize) * crossChance));

        return newSize;
    }

    public void InitializeFromParents(Genetics parent1, Genetics parent2) 
    {
        float crossAt = Random.value;
        int newSize = CrossValue(parent1.GetDataSize(), parent2.GetDataSize());
        int newTSize = CrossValue(parent1.GetTraitCount(), parent2.GetTraitCount());

        m_Data = new byte[newSize];
        m_Traits = new GeneticTrait[newTSize];
        m_Parents = new Genetics[] { parent1, parent2 };

        // Crossover
        if(m_CrossoverRate > 0 && crossAt <= m_CrossoverRate) {
            int crossDSize = (int)(newSize * crossAt);
            int crossTSize = (int)(newTSize * crossAt);
            
            // 50% chance one of the parents
            Genetics parent = crossAt > 0.5f
              ? parent2 
              : parent1;

            for(int i = 0; i < newSize; i++) {
                m_Data[i] = parent.GetData().ElementAt(i);
            }
            for(int i = crossTSize; i < newTSize; i++) {
                GeneticTrait crossTrait = parent.GetTrait((GeneticTraitType)i);
                m_Traits[i] = new GeneticTrait(crossTrait.Identifier, crossTrait.Start, crossTrait.End);
            }

            SwapWith(parent2, crossDSize);
        }

        // Mutate
        for(int i = 0; i < m_Data.Length; i++) 
        {
            if(Random.value <= m_MutationRate)
              m_Data[i] = (byte)Random.Range(0, 255);
        }

        // Assign mutation to traits, ensure we do not overlap with the other traits
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
        return m_Traits[(int)tt];
    }

    public IEnumerable<byte> GetDataForTrait(GeneticTrait t)
    {
        return m_Data
            .Skip(t.Start)
            .Take(t.Count());
    }

    public IEnumerable<byte> GetData() => m_Data;
    public int GetDataSize() => m_Data.Length;
    public int GetTraitCount() => m_Traits.Length;
}
