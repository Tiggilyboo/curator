using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Assets/Resource/New Resource")]
public class Resource: ScriptableObject
{
    public delegate void ResourceChangeDelegate(Resource r);
    public event ResourceChangeDelegate OnResourceChange;

    public delegate void ResourceEmptyDelegate(Resource r);
    public event ResourceEmptyDelegate OnResourceEmpty;

    [SerializeField]
    private string m_Identifier;
    [SerializeField]
    private int m_Quantity;
    [SerializeField]
    private bool m_Edible;

    [SerializeField]
    private Texture2D m_Image;

    public string Identifier => m_Identifier;
    public bool IsEdible => m_Edible;

    public int Quantity 
    { 
      get => m_Quantity; 
      set 
      { 
          m_Quantity = value; 

          if(m_Quantity != 0)
            TriggerChange();
          else
            OnResourceEmpty?.Invoke(this);
      }
    }

    public Texture2D Image => m_Image;

    public void TriggerChange() 
    {
        OnResourceChange?.Invoke(this);
    }
} 
