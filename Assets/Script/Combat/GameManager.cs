using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MonsterSpawner monsterSpawner;

    void Start()
    {
        // ���� (ID, ����) Ʃ�� ���� ����
        (int monsterId, int level) monsterInfo = (1, 5);
        monsterSpawner.SpawnMonster(monsterInfo);
    }
}