using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject Buff_Effect;
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
    }

    private void Win()
    {
        GameData.ClearTargetNode();
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("�÷��̾� �� ����: ī�� ȿ�� ���� �� �ִϸ��̼� ����");
        StartCoroutine(ExecutePlayerTurn());
    }

    private void ApplyBuffImage(Transform targetTransform, Sprite buffSprite)
    {
        GameObject buffImage = Instantiate(Buff_Effect, Vector3.zero, Quaternion.identity);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("UI ĵ������ ã�� �� �����ϴ�.");
            return;
        }
        buffImage.transform.SetParent(canvas.transform, false);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position + Vector3.up * 2);

        RectTransform buffRect = buffImage.GetComponent<RectTransform>();
        if (buffRect != null)
        {
            buffRect.position = screenPosition;
        }
        Image buffImageComponent = buffImage.GetComponent<Image>();
        if (buffImageComponent != null)
        {
            buffImageComponent.sprite = buffSprite;
        }
        Destroy(buffImage, 2.0f);
    }
    private void ApplyBuffImage(Vector3 position, Sprite buffSprite)
    {
        GameObject buffImage = Instantiate(Buff_Effect, position, Quaternion.identity);
        Image buffImageComponent = buffImage.GetComponent<Image>();

        if (buffImageComponent != null)
        {
            buffImageComponent.sprite = buffSprite;
        }

        Destroy(buffImage, 2.0f);
    }


    private IEnumerator ExecutePlayerTurn()
    {
        foreach (var card in deckManager.hand)
        {
            if (card == null) continue;

            bool effectApplied = false;

            playerBattleSystem.GetComponent<Player>().PlayAnimationAndApplyEffect(card, () =>
            {
                // �ִϸ��̼��� ���� �� ī�� ȿ�� ����
                switch (card.cardType)
                {
                    case CardType.Attack:
                        ApplyCardEffectToMonsters(card);
                        break;

                    case CardType.Defense:

                    case CardType.Skill:
                        ApplyCardEffectToPlayer(card);
                        break;
                }

                effectApplied = true;
            });

            // ī�� ȿ���� ����� ������ ���
            while (!effectApplied)
            {
                yield return null;
            }
        }

        // �� ���� �� ī�� �ʱ�ȭ
        deckManager.hand.Clear();
        deckManager.RerollCards();

        // ���� ������ ��ȯ
       // StartCoroutine(MonsterTurn());
    }


    private IEnumerator MonsterTurn()
    {
        //Loading.LoadScene(Scene.WORLDMAP);
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
                        monster.OnDamage(DeckManager.Atp * 10); // ī�� ������ ����
                        Debug.Log($"{monster.name}��(��) {DeckManager.Atp * 10}�� ���ظ� �Ծ����ϴ�.");
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
                playerBattleSystem.Armor += DeckManager.Dep * 5; // ���� ����
                Debug.Log($"�÷��̾� ������ {DeckManager.Dep * 5}��ŭ �����߽��ϴ�.");
                ApplyBuffImage(playerBattleSystem.transform, Def_Effect);
                break;

            case CardType.Skill:
                if ((playerBattleSystem.curHp += DeckManager.Sp *10)<= playerBattleSystem.maxHp) {
                    playerBattleSystem.curHp += DeckManager.Sp * 10; // ü�� ȸ��
                    Debug.Log($"�÷��̾� ü���� {DeckManager.Sp * 10}��ŭ ȸ���Ǿ����ϴ�.");
                    ApplyBuffImage(playerBattleSystem.transform,Heal_Effect);
                }
                else
                {
                    playerBattleSystem.curHp = playerBattleSystem.maxHp;
                    Debug.Log($"�÷��̾��� ü���� ���� á���ϴ�.");
                    ApplyBuffImage(playerBattleSystem.transform, Heal_Effect);
                }
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
