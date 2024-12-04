using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour
{
    public BattleSystem playerBattleSystem; // �÷��̾� BattleSystem
    public DeckManager deckManager; // �� �Ŵ���
    public Transform[] monsterSpawnPoints; // ���� ���� ��ġ �迭
    public List<MonsterData> monsterDatas; // ���� ������ ����Ʈ
    public MonsterGen monsterGen; // ���� ������ ����
    public List<Monster> monsters = new List<Monster>(); // ���� ���������� ���� ����Ʈ
    public bool isBattleOver = false; // ���� ���� ���� Ȯ��
    public Sprite Def_Effect;
    public Sprite Heal_Effect;


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

        if (monsters.Count > 0)
        {
            playerBattleSystem.Target = monsters[0].gameObject; // ù ��° ���͸� Ÿ������ ����
        }

        foreach (var monster in monsters)
        {
            if (monster.Target == null)
            {
                monster.Target = playerBattleSystem.gameObject; // ������ Ÿ�� ����
            }
        }
    }
    private void Update()
    {
        // ���� ���¸� �� ������ Ȯ��
        if (!isBattleOver && CheckAllMonstersDead())
        {
            Win();
        }
    }

    private bool CheckAllMonstersDead()
    {
        foreach (var monster in monsters)
        {
            if (monster.IsLive) // ����ִ� ���Ͱ� �ִٸ�
            {
                return false;
            }
        }
        return true; // ��� ���Ͱ� �׾��ٸ� true ��ȯ
    }

    private void Win()
    {
        isBattleOver = true; // �ߺ� ȣ�� ����

        GameData.ClearTargetNode();
        Debug.Log("�¸�! 5�� �Ŀ� ���� ������ ��ȯ�˴ϴ�.");
        StartCoroutine(DelayedSceneTransition());
    }
    private IEnumerator DelayedSceneTransition()
    {
        yield return new WaitForSeconds(5.0f);
        Loading.LoadScene(Scene.WORLDMAP);
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("�÷��̾� �� ����: ī�� ȿ�� ���� �� �ִϸ��̼� ����");
        StartCoroutine(ExecutePlayerTurn());
    }

    private IEnumerator ExecutePlayerTurn()
    {
        Card[] handCards = deckManager.hand.ToArray();
        Monster targetMonster = monsters[Random.Range(0, monsters.Count)];

        if (handCards.Length == 0 || targetMonster == null)
        {
            Debug.LogWarning("�÷��̾� �ڵ尡 ��� �ְų� ���Ͱ� �����ϴ�.");
            yield break;
        }

        bool allEffectsApplied = false;

        playerBattleSystem.GetComponent<Player>().PlayCardsSequentially(handCards, targetMonster, () =>
        {
            allEffectsApplied = true; // ��� ī�� ȿ�� ���� �Ϸ� ��ȣ
        });

        while (!allEffectsApplied)
        {
            yield return null; // ī�� ȿ�� ���� �Ϸ� ���
        }

        // �� ���� �� ī�� �ʱ�ȭ
        deckManager.hand.Clear();
        deckManager.RerollCards();

        // ���� �� ����
        StartCoroutine(MonsterTurn());
    }

    private IEnumerator MonsterTurn()
    {
        Debug.Log("[MonsterTurn] ���� �� ����");
        isPlayerTurn = false;

        foreach (var monster in monsters)
        {
            if (monster.IsLive)
            {
                Debug.Log($"[MonsterTurn] {monster.name}��(��) �÷��̾ �����մϴ�.");

                if (monster.Target == null)
                {
                    Debug.LogError($"[MonsterTurn] ���� {monster.name}�� Target�� null�Դϴ�!");
                    monster.Target = playerBattleSystem.gameObject; // �÷��̾ Ÿ������ ����
                }

                if (monster.Target.TryGetComponent<IDamage>(out IDamage damageable))
                {
                    monster.OnAttack(); // ���� ����
                }
                else
                {
                    Debug.LogError($"[MonsterTurn] {monster.Target.name}���� IDamage ������Ʈ�� �����ϴ�!");
                }

                yield return new WaitForSeconds(1.0f); // ���� �� ��� �ð�
            }
        }

        Debug.Log("[MonsterTurn] ���� �� ����: �÷��̾� �� ����");
        isPlayerTurn = true;
    }



    private void ApplyCardEffect(Card card, Monster targetMonster)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                targetMonster.OnDamage(card.effectValue); // ���� ī���� ȿ�� ����ŭ ������
                Debug.Log($"{targetMonster.name}���� {card.effectValue}�� �������� �������ϴ�.");
                break;

            case CardType.Defense:
                playerBattleSystem.Armor += card.effectValue; // ���� ����
                Debug.Log($"�÷��̾� ������ {card.effectValue}��ŭ �����߽��ϴ�.");
                break;

            case CardType.Skill:
                playerBattleSystem.curHp = Mathf.Min(playerBattleSystem.curHp + card.effectValue, playerBattleSystem.maxHp); // ü�� ȸ��
                Debug.Log($"�÷��̾� ü���� {card.effectValue}��ŭ ȸ���Ǿ����ϴ�. ���� ü��: {playerBattleSystem.curHp}");
                break;


            default:
                Debug.LogWarning($"�� �� ���� ī�� Ÿ��: {card.cardType}");
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
                monster.Initialize(monsterInfo.Item1, monsterInfo.Item2, data, playerBattleSystem.gameObject);
                monsters.Add(monster);
                Debug.Log($"���� ����: {data.monsterName} (Lv {monsterInfo.Item2})");
                monsterObject.transform.LookAt(playerBattleSystem.transform.position);
            }
                if (GameData.targetNode.type == CheonJiWoon.Node.Type.END)
            {
                monster.gameObject.transform.localScale = new Vector3(1, 1, 1) * 3.0f;
            }
                if (GameData.targetNode.type == CheonJiWoon.Node.Type.END&&
                monsterInfo.Item1 == 3)
            {
                monster.gameObject.transform.localScale = new Vector3(1, 1, 1) * 1.5f;
            }
                if (GameData.targetNode.type == CheonJiWoon.Node.Type.END &&
                monsterInfo.Item1 == 5)
            {
                monster.gameObject.transform.localScale = new Vector3(1, 1, 1) * 2.0f;
            }
        }
    }

}
