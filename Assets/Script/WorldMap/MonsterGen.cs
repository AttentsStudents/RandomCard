using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGen : MonoBehaviour
{
    public static MonsterGen instance;

    public GameObject[] list;
    void Awake() => instance = this;

    public GameObject RandomMonster()
    {
        return instance.list[Random.Range(0, instance.list.Length)];
    }
}
