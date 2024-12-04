using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public bool isBattleOver = false; // 전투 종료 여부 확인
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

        if (monsters.Count > 0)
        {
            playerBattleSystem.Target = monsters[0].gameObject; // 첫 번째 몬스터를 타겟으로 설정
        }

        foreach (var monster in monsters)
        {
            if (monster.Target == null)
            {
                monster.Target = playerBattleSystem.gameObject; // 몬스터의 타겟 설정
            }
        }
    }
    private void Update()
    {
        // 몬스터 상태를 매 프레임 확인
        if (!isBattleOver && CheckAllMonstersDead())
        {
            Win();
        }
    }

    private bool CheckAllMonstersDead()
    {
        foreach (var monster in monsters)
        {
            if (monster.IsLive) // 살아있는 몬스터가 있다면
            {
                return false;
            }
        }
        return true; // 모든 몬스터가 죽었다면 true 반환
    }

    private void Win()
    {
        isBattleOver = true; // 중복 호출 방지

        GameData.ClearTargetNode();
        Debug.Log("승리! 5초 후에 다음 씬으로 전환됩니다.");
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

        Debug.Log("플레이어 턴 종료: 카드 효과 적용 및 애니메이션 실행");
        StartCoroutine(ExecutePlayerTurn());
    }

    private IEnumerator ExecutePlayerTurn()
    {
        Card[] handCards = deckManager.hand.ToArray();
        Monster targetMonster = monsters[Random.Range(0, monsters.Count)];

        if (handCards.Length == 0 || targetMonster == null)
        {
            Debug.LogWarning("플레이어 핸드가 비어 있거나 몬스터가 없습니다.");
            yield break;
        }

        bool allEffectsApplied = false;

        playerBattleSystem.GetComponent<Player>().PlayCardsSequentially(handCards, targetMonster, () =>
        {
            allEffectsApplied = true; // 모든 카드 효과 적용 완료 신호
        });

        while (!allEffectsApplied)
        {
            yield return null; // 카드 효과 적용 완료 대기
        }

        // 턴 종료 후 카드 초기화
        deckManager.hand.Clear();
        deckManager.RerollCards();

        // 몬스터 턴 시작
        StartCoroutine(MonsterTurn());
    }

    private IEnumerator MonsterTurn()
    {
        Debug.Log("[MonsterTurn] 몬스터 턴 시작");
        isPlayerTurn = false;

        foreach (var monster in monsters)
        {
            if (monster.IsLive)
            {
                Debug.Log($"[MonsterTurn] {monster.name}이(가) 플레이어를 공격합니다.");

                if (monster.Target == null)
                {
                    Debug.LogError($"[MonsterTurn] 몬스터 {monster.name}의 Target이 null입니다!");
                    monster.Target = playerBattleSystem.gameObject; // 플레이어를 타겟으로 설정
                }

                if (monster.Target.TryGetComponent<IDamage>(out IDamage damageable))
                {
                    monster.OnAttack(); // 공격 실행
                }
                else
                {
                    Debug.LogError($"[MonsterTurn] {monster.Target.name}에는 IDamage 컴포넌트가 없습니다!");
                }

                yield return new WaitForSeconds(1.0f); // 공격 간 대기 시간
            }
        }

        Debug.Log("[MonsterTurn] 몬스터 턴 종료: 플레이어 턴 시작");
        isPlayerTurn = true;
    }



    private void ApplyCardEffect(Card card, Monster targetMonster)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                targetMonster.OnDamage(card.effectValue); // 공격 카드의 효과 값만큼 데미지
                Debug.Log($"{targetMonster.name}에게 {card.effectValue}의 데미지를 입혔습니다.");
                break;

            case CardType.Defense:
                playerBattleSystem.Armor += card.effectValue; // 방어력 증가
                Debug.Log($"플레이어 방어력이 {card.effectValue}만큼 증가했습니다.");
                break;

            case CardType.Skill:
                playerBattleSystem.curHp = Mathf.Min(playerBattleSystem.curHp + card.effectValue, playerBattleSystem.maxHp); // 체력 회복
                Debug.Log($"플레이어 체력이 {card.effectValue}만큼 회복되었습니다. 현재 체력: {playerBattleSystem.curHp}");
                break;


            default:
                Debug.LogWarning($"알 수 없는 카드 타입: {card.cardType}");
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
