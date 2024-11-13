using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;

    public Monster SpawnMonster((int monsterId, int level) monsterInfo)
    {
        GameObject monsterObject = Instantiate(monsterPrefab);
        Monster monster = monsterObject.GetComponent<Monster>();

        if (monster != null)
        {
            monster.Initialize(monsterInfo.monsterId, monsterInfo.level);
            monster.DisplayStats(); // 몬스터의 스탯을 출력하여 확인
        }

        return monster;
    }
}
