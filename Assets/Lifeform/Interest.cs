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
    [SerializeField]
    private Intent m_Intent;
    [SerializeField]
    private Vector3 m_Point;
    [SerializeField]
    private GameObject m_Object;
    
    public Intent Intent => m_Intent;
    public Vector3 Point => m_Point;

    public Interest(Intent intent)
    {
        m_Intent = intent;
    }

    public Interest(Intent intent, Vector3 point)
      : this(intent)
    {
        m_Point = point;
    }

    public Interest(Intent intent, GameObject obj)
      : this(intent)
    {
        m_Object = obj;
    }
}
