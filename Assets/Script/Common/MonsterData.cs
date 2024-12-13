using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Scriptable Object/Monster Data", order = 1)]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public float maxHP;
    public float armor;
    public float attack;
    public float levelStatAdd;
}
