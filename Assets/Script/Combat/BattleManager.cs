using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleSystem playerBattleSystem; // �÷��̾� BattleSystem
    public DeckManager deckManager; // �� �Ŵ���
    public Transform[] monsterSpawnPoints; // ���� ���� ��ġ �迭
    public List<MonsterData> monsterDatas; // ���� ������ ����Ʈ
    public MonsterGen monsterGen; // ���� ������ ����
    public List<Monster> monsters = new List<Monster>(); // ���� ���������� ���� ����Ʈ

    private bool isPlayerTurn = true; // �÷��̾� �� �÷���

    private void Start()
    {
        if (playerBattleSystem == null)
            Debug.LogError("PlayerBattleSystem�� ������� �ʾҽ��ϴ�.");

        if (deckManager == null)
            Debug.LogError("DeckManager�� ������� �ʾҽ��ϴ�.");

        if (monsterGen == null)
            Debug.LogError("MonsterGen�� ������� �ʾҽ��ϴ�.");

        SpawnMonsters(); // ���� ����
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("�÷��̾� �� ����: ī�� ȿ�� ���� �� �ִϸ��̼� ����");
        StartCoroutine(ExecutePlayerTurn());
    }

    private IEnumerator ExecutePlayerTurn()
    {
        foreach (var card in deckManager.hand)
        {
            if (card == null) continue;

            switch (card.cardType)
            {
                case CardType.Attack:
                    playerBattleSystem.TriggerAnimation(playerBattleSystem.animData.OnAttack);
                    yield return new WaitForSeconds(1.0f); // �ִϸ��̼� ���
                    ApplyCardEffectToMonsters(card);
                    break;

                case CardType.Defense:
                    playerBattleSystem.TriggerAnimation(playerBattleSystem.animData.OnDamage);
                    yield return new WaitForSeconds(1.0f); // �ִϸ��̼� ���
                    ApplyCardEffectToPlayer(card);
                    break;

                case CardType.Skill:
                    playerBattleSystem.TriggerAnimation(playerBattleSystem.animData.OnSkill);
                    yield return new WaitForSeconds(1.0f); // �ִϸ��̼� ���
                    ApplyCardEffectToPlayer(card);
                    break;
            }
        }

        // �� ���� �� ī�� �ʱ�ȭ
        deckManager.hand.Clear();
        deckManager.RerollCards();

        // ���� ������ ��ȯ
        StartCoroutine(MonsterTurn());
    }

    private IEnumerator MonsterTurn()
    {
        Loading.LoadScene(Scene.WORLDMAP);
        Debug.Log("���� �� ����");
        isPlayerTurn = false;

        foreach (var monster in monsters)
        {
            if (monster.IsLive)
            {
                Debug.Log($"{monster.name}��(��) �÷��̾ �����մϴ�.");
                monster.OnAttack(); // ���� ����
                yield return new WaitForSeconds(1.0f); // ���� ���
            }
        }

        Debug.Log("���� �� ����: �÷��̾� �� ����");
        isPlayerTurn = true;
    }

    private void ApplyCardEffectToMonsters(Card card)
    {
        foreach (var monster in monsters)
        {
            if (monster.IsLive)
            {
                switch (card.cardType)
                {
                    case CardType.Attack:
                        monster.OnDamage(card.energyCost * 10); // ī�� ������ ����
                        Debug.Log($"{monster.name}��(��) {card.energyCost * 10}�� ���ظ� �Ծ����ϴ�.");
                        break;
                }
            }
        }
    }

    private void ApplyCardEffectToPlayer(Card card)
    {
        switch (card.cardType)
        {
            case CardType.Defense:
                playerBattleSystem.Armor += card.energyCost * 5; // ���� ����
                Debug.Log($"�÷��̾� ������ {card.energyCost * 5}��ŭ �����߽��ϴ�.");
                break;

            case CardType.Skill:
                playerBattleSystem.curHp += card.energyCost * 10; // ü�� ȸ��
                Debug.Log($"�÷��̾� ü���� {card.energyCost * 10}��ŭ ȸ���Ǿ����ϴ�.");
                break;
        }
    }

    /*
       foreach(var monsterInfo in GameData.enemies)
        {
            MonsterGen.instance.list[monsterInfo.Item1];
        }
*/
    public void SpawnMonsters()
    {
        if (GameData.enemies == null || GameData.enemies.Count == 0)
        {
            Debug.LogError("GameData.enemies�� ��� �ֽ��ϴ�!");
            return;
        }

        int monsterCount = Mathf.Min(GameData.enemies.Count, monsterSpawnPoints.Length);
        for (int i = 0; i < monsterCount; i++)
        {
            var monsterInfo = GameData.enemies[i];

            GameObject monsterPrefab = MonsterGen.instance.list[monsterInfo.Item1];
            GameObject monsterObject = Instantiate(monsterPrefab, monsterSpawnPoints[i].position, Quaternion.identity);

            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null)
            {
                MonsterData data = monsterDatas[monsterInfo.Item1];
                monster.Initialize(monsterInfo.Item1, monsterInfo.Item2, data);
                monsters.Add(monster);
                Debug.Log($"���� ����: {data.monsterName} (Lv {monsterInfo.Item2})");
            }
        }
    }

}
