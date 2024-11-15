using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    private List<Monster> monsterInstances = new List<Monster>();

    public void SpawnMonsters()
    {
        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            GameObject monsterObject = Instantiate(monsterPrefabs[i]);
            Monster monster = monsterObject.GetComponent<Monster>();

            if (monster != null)
            {
                int level = Random.Range(1, 10);
                monster.Initialize(i + 1, level);
                monsterInstances.Add(monster);
            }
        }
    }

    public void DisplayAllMonsterStats()
    {
        foreach (var monster in monsterInstances)
        {
            monster.DisplayStats();
        }
    }
}
