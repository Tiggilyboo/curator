using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Assets/Resource/New Resource")]
public class Resource: ScriptableObject, IInteractable
{
    public delegate void ResourceChangeDelegate(Resource r);
    public event ResourceChangeDelegate OnResourceChange;

    public delegate void ResourceEmptyDelegate(Resource r);
    public event ResourceEmptyDelegate OnResourceEmpty;

    [SerializeField]
    private string m_Identifier;
    [SerializeField]
    private float m_Quantity;
    [SerializeField]
    private bool m_Edible;
    [SerializeField]
    private Texture2D m_Image;

    public string Identifier => m_Identifier;
    public bool IsEdible => m_Edible;

    public float Quantity 
    { 
      get => m_Quantity; 
      set 
      { 
          m_Quantity = value; 

          if(m_Quantity > 0)
            TriggerChange();
          else
            OnResourceEmpty?.Invoke(this);
      }
    }

    public Texture2D Image => m_Image;
    
    private void ConsumeEdible(Lifeform lf)
    {
        float maximumIncrease = lf.Genetics.GetMaxHunger() - lf.Hunger;
        if(Quantity > maximumIncrease)
        {
            lf.DeltaHunger(maximumIncrease);
            Quantity -= maximumIncrease;
        }
        else
        {
            lf.DeltaHunger(Quantity);
            Quantity = 0;
        }
    }

    private void HandleEmpty()
    {
        OnResourceEmpty?.Invoke(this);
    }

    public void Interact(Lifeform lf)
    {
        if(!IsEdible)
          throw new NotImplementedException();

        ConsumeEdible(lf);
    }

    public void TriggerChange() 
    {
        OnResourceChange?.Invoke(this);
    }
} 
