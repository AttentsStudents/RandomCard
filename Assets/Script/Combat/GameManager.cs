using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MonsterSpawner monsterSpawner;

    void Start()
    {
        // 몬스터 (ID, 레벨) 튜플 생성 예시
        (int monsterId, int level) monsterInfo = (1, 5);
        monsterSpawner.SpawnMonster(monsterInfo);
    }
}