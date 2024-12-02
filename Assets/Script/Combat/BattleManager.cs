using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour
{
    public BattleSystem playerBattleSystem; // 플레이어 BattleSystem
    public DeckManager deckManager; // 덱 매니저
    public Transform[] monsterSpawnPoints; // 몬스터 스폰 위치 배열
    public List<MonsterData> monsterDatas; // 몬스터 데이터 리스트
    public MonsterGen monsterGen; // 몬스터 프리팹 관리
    public List<Monster> monsters = new List<Monster>(); // 현재 스테이지의 몬스터 리스트
    public GameObject Buff_Effect;
    public Sprite Def_Effect;
    public Sprite Heal_Effect;



    private bool isPlayerTurn = true; // 플레이어 턴 플래그

    private void Start()
    {
        if (playerBattleSystem == null)
            Debug.LogError("PlayerBattleSystem이 연결되지 않았습니다.");

        if (deckManager == null)
            Debug.LogError("DeckManager가 연결되지 않았습니다.");

        if (monsterGen == null)
            Debug.LogError("MonsterGen이 연결되지 않았습니다.");

        SpawnMonsters(); // 몬스터 생성
    }

    private void Win()
    {
        GameData.ClearTargetNode();
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("플레이어 턴 종료: 카드 효과 적용 및 애니메이션 실행");
        StartCoroutine(ExecutePlayerTurn());
    }

    private void ApplyBuffImage(Transform targetTransform, Sprite buffSprite)
    {
        GameObject buffImage = Instantiate(Buff_Effect, Vector3.zero, Quaternion.identity);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("UI 캔버스를 찾을 수 없습니다.");
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
                // 애니메이션이 끝난 후 카드 효과 적용
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

            // 카드 효과가 적용될 때까지 대기
            while (!effectApplied)
            {
                yield return null;
            }
        }

        // 턴 종료 후 카드 초기화
        deckManager.hand.Clear();
        deckManager.RerollCards();

        // 몬스터 턴으로 전환
       // StartCoroutine(MonsterTurn());
    }


    private IEnumerator MonsterTurn()
    {
        //Loading.LoadScene(Scene.WORLDMAP);
        Debug.Log("몬스터 턴 시작");
        isPlayerTurn = false;

        foreach (var monster in monsters)
        {
            if (monster.IsLive)
            {
                Debug.Log($"{monster.name}이(가) 플레이어를 공격합니다.");
                monster.OnAttack(); // 몬스터 공격
                yield return new WaitForSeconds(1.0f); // 공격 대기
            }
        }

        Debug.Log("몬스터 턴 종료: 플레이어 턴 시작");
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
                        monster.OnDamage(DeckManager.Atp * 10); // 카드 데미지 적용
                        Debug.Log($"{monster.name}이(가) {DeckManager.Atp * 10}의 피해를 입었습니다.");
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
                playerBattleSystem.Armor += DeckManager.Dep * 5; // 방어력 증가
                Debug.Log($"플레이어 방어력이 {DeckManager.Dep * 5}만큼 증가했습니다.");
                ApplyBuffImage(playerBattleSystem.transform, Def_Effect);
                break;

            case CardType.Skill:
                if ((playerBattleSystem.curHp += DeckManager.Sp *10)<= playerBattleSystem.maxHp) {
                    playerBattleSystem.curHp += DeckManager.Sp * 10; // 체력 회복
                    Debug.Log($"플레이어 체력이 {DeckManager.Sp * 10}만큼 회복되었습니다.");
                    ApplyBuffImage(playerBattleSystem.transform,Heal_Effect);
                }
                else
                {
                    playerBattleSystem.curHp = playerBattleSystem.maxHp;
                    Debug.Log($"플레이어의 체력이 가득 찼습니다.");
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
            Debug.LogError("GameData.enemies가 비어 있습니다!");
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
                Debug.Log($"몬스터 생성: {data.monsterName} (Lv {monsterInfo.Item2})");
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
