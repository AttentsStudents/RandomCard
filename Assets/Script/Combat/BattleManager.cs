using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleSystem playerBattleSystem; // 플레이어 BattleSystem
    public DeckManager deckManager; // 덱 매니저
    public Transform[] monsterSpawnPoints; // 몬스터 스폰 위치 배열
    public List<MonsterData> monsterDatas; // 몬스터 데이터 리스트
    public MonsterGen monsterGen; // 몬스터 프리팹 관리
    public List<Monster> monsters = new List<Monster>(); // 현재 스테이지의 몬스터 리스트

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

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("플레이어 턴 종료: 카드 효과 적용 및 애니메이션 실행");
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
                    yield return new WaitForSeconds(1.0f); // 애니메이션 대기
                    ApplyCardEffectToMonsters(card);
                    break;

                case CardType.Defense:
                    playerBattleSystem.TriggerAnimation(playerBattleSystem.animData.OnDamage);
                    yield return new WaitForSeconds(1.0f); // 애니메이션 대기
                    ApplyCardEffectToPlayer(card);
                    break;

                case CardType.Skill:
                    playerBattleSystem.TriggerAnimation(playerBattleSystem.animData.OnSkill);
                    yield return new WaitForSeconds(1.0f); // 애니메이션 대기
                    ApplyCardEffectToPlayer(card);
                    break;
            }
        }

        // 턴 종료 후 카드 초기화
        deckManager.hand.Clear();
        deckManager.RerollCards();

        // 몬스터 턴으로 전환
        StartCoroutine(MonsterTurn());
    }

    private IEnumerator MonsterTurn()
    {
        Loading.LoadScene(Scene.WORLDMAP);
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
                        monster.OnDamage(card.energyCost * 10); // 카드 데미지 적용
                        Debug.Log($"{monster.name}이(가) {card.energyCost * 10}의 피해를 입었습니다.");
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
                playerBattleSystem.Armor += card.energyCost * 5; // 방어력 증가
                Debug.Log($"플레이어 방어력이 {card.energyCost * 5}만큼 증가했습니다.");
                break;

            case CardType.Skill:
                playerBattleSystem.curHp += card.energyCost * 10; // 체력 회복
                Debug.Log($"플레이어 체력이 {card.energyCost * 10}만큼 회복되었습니다.");
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
                monster.Initialize(monsterInfo.Item1, monsterInfo.Item2, data);
                monsters.Add(monster);
                Debug.Log($"몬스터 생성: {data.monsterName} (Lv {monsterInfo.Item2})");
            }
        }
    }

}
