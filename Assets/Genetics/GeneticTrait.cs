using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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
public class GeneticTrait: ICloneable
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

        // Otherwise, create empty bytes to operate upon.
        byte[] newSegment = geneSegment
            .Take(size)
            .ToArray();

        return converter.Invoke(newSegment, 0);
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

    public object Clone() 
    {
        return new GeneticTrait(m_Type, m_Start, m_End);
    }
}
