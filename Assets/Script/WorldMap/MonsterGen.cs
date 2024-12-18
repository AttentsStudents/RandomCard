using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGen : SingleTon<MonsterGen>
{
    public GameObject[] list;
    void Awake() => Init();

    public GameObject RandomMonster() => inst.list[Random.Range(0, inst.list.Length)];
}
