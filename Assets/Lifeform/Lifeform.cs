using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifeform : MonoBehaviour
{
    public bool Moving;
    public bool Eating;
    public bool Sleeping;

    public float Hunger;
    public float Age;
    public float Energy;
    
    // To later be derived from genetics
    public float Eyesight = 10.0f;
    public float MaxHunger = 5.0f;
    public float MaxAge = 50.0f;
    public float MaxEnergy = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
      Hunger = MaxHunger;
      Energy = MaxEnergy;
    }

    float MoveRate() 
    {
      return Moving 
        ? 0.003f
        : 0.001f;
    }

    // Update is called once per frame
    void Update()
    {
      Energy -= MoveRate();
      Hunger -= 0.00001f;
      Age += 0.0001f;
    }
}
