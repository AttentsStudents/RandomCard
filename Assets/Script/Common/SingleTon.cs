using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T inst { get; protected set; }
    protected void Init()
    {
        if (inst == null)
        {
            inst = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
