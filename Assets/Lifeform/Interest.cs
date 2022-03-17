using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Intent 
{
  Idle,
  Wander,
  Eat,
  Sleep
}

public class Interest 
{
    public Vector3 Point;
    public GameObject Object;
    public Intent Intent;
    public float Dedication;

    public static Interest Wander(Vector3 point) 
    {
        return new Interest 
        {
            Point = point,
            Intent = Intent.Wander,
            Dedication = 10.0f,
        };
    }

    public static Interest Idle() {
        return new Interest 
        {
            Intent = Intent.Idle,
            Dedication = 1.0f,
        };
    }
}
