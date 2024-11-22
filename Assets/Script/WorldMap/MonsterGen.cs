using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGen : MonoBehaviour
{
    public static MonsterGen instance;

    public GameObject[] list;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        if (instance != this) DestroyImmediate(this);
    }

    public GameObject RandomMonster()
    {
        return instance.list[Random.Range(0, instance.list.Length)];
    }
}
