using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : BattleSystem, IDamage
{
    public Transform targetMonster;
    private NavMeshAgent navMeshAgent;
    public Animator anim;
    public float attackRange = 2.0f; // 타겟과의 최소 거리
    public PlayerData playerData;

    private Vector3 originalPosition; // 원래 위치
    private bool isMovingToTarget = false; // 이동 중인지 확인

    void Start()
    {
        OnReset();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent가 필요합니다.");
        }
        InitializePlayerStats();
    }

    private void InitializePlayerStats()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData가 연결되지 않았습니다.");
            return;
        }
        battleStat = new BattleStat(playerData.maxHP, playerData.armor, playerData.attack);
        curHp = battleStat.maxHP; // 현재 체력을 최대 체력으로 설정
    }

    public void PlayCardsSequentially(Card[] cards, Monster[] targetMonsters, System.Action onAllEffectsComplete)
    {
        originalPosition = transform.position; // 플레이어 초기 위치 저장
        StartCoroutine(ExecuteCardsSequentially(cards, targetMonsters, onAllEffectsComplete));
    }

    private IEnumerator ExecuteCardsSequentially(Card[] cards, Monster[] targetMonsters, System.Action onAllEffectsComplete)
    {
        if (cards == null || targetMonsters == null || cards.Length == 0)
        {
            Debug.LogError("카드 배열 또는 타겟 몬스터 배열이 비어 있습니다.");
            onAllEffectsComplete?.Invoke();
            yield break;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            Monster targetMonster = targetMonsters[i % targetMonsters.Length];
            Debug.Log($"카드 {i + 1}/{cards.Length} 실행 중, 타겟: {targetMonster.name}");

            // 타겟 몬스터 앞으로 이동
            yield return MoveToTargetAndExecuteAction(targetMonster.transform, () =>
            {
                Debug.Log($"카드 {i + 1}: 타겟 {targetMonster.name} 근처 도착");
            });

            // 카드 효과 실행
            bool effectApplied = false;
            StartCoroutine(PlayAnimationAndEffect(cards[i], targetMonster, () =>
            {
                Debug.Log($"카드 {i + 1}: 효과 적용 완료");
                effectApplied = true;
            }));

            // 효과 적용 완료 대기
            while (!effectApplied)
            {
                yield return null;
            }

            // 원래 위치로 복귀
            Debug.Log($"카드 {i + 1}: 효과 완료 후 원래 위치로 복귀 중");
            yield return ReturnToOriginalPosition(() =>
            {
                Debug.Log($"카드 {i + 1}: 복귀 완료");
            });
        }

        // 모든 카드 효과 실행 완료
        Debug.Log("모든 카드 효과 실행 완료");
        onAllEffectsComplete?.Invoke();
    }


    private IEnumerator PlayAnimationAndEffect(Card card, Monster targetMonster, System.Action onEffectComplete)
    {
        // 카드 타입에 따른 애니메이션 실행
        switch (card.cardType)
        {
            case CardType.Attack:
                myAnim.SetTrigger(animData.OnAttack);
                break;

            case CardType.Defense:
                myAnim.SetTrigger(animData.OnDamage);
                break;

            case CardType.Skill:
                myAnim.SetTrigger(animData.OnSkill);
                break;
        }

        // 애니메이션 상태 확인 및 대기
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);
        Debug.Log($"애니메이션 종료: {card.cardType}");

        ApplyCardEffect(card, targetMonster);

        Debug.Log("카드 효과 적용 완료");
        onEffectComplete?.Invoke();
    }


    private IEnumerator MoveToTargetAndExecuteAction(Transform targetTransform, System.Action onArrived)
    {
        if (navMeshAgent == null || targetTransform == null)
        {
            Debug.LogError("NavMeshAgent 또는 타겟 Transform이 null입니다.");
            yield break;
        }

        navMeshAgent.SetDestination(targetTransform.position);

        // 타겟 위치에 도달할 때까지 대기
        while (Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        onArrived?.Invoke(); // 타겟 근처 도착 콜백 실행
    }


    private IEnumerator ReturnToOriginalPosition(System.Action onReturned)
    {
        if (originalPosition == null)
        {
            Debug.LogError("원래 위치 정보가 null입니다.");
            yield break;
        }

        navMeshAgent.SetDestination(originalPosition);

        // 원래 위치로 이동
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        onReturned?.Invoke(); // 복귀 완료 콜백 실행
    }


    private void PlayAnimation(Card card)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                myAnim.SetTrigger(animData.OnAttack);
                break;

            case CardType.Defense:
                myAnim.SetTrigger(animData.OnDamage);
                break;

            case CardType.Skill:
                myAnim.SetTrigger(animData.OnSkill);
                break;
        }
    }

    private void ApplyCardEffect(Card card, Monster targetMonster)
    {
        if (card.cardType == CardType.Attack)
        {
            targetMonster.OnDamage(battleStat.Attak * 10); // 공격력 기반 데미지 적용
            Debug.Log($"[카드 효과] {targetMonster.name}에게 {battleStat.Attak * 10}의 피해를 입혔습니다.");
        }
        else if (card.cardType == CardType.Defense)
        {
            Armor += 5; // 방어력 증가
            Debug.Log($"[카드 효과] 플레이어의 방어력이 5 증가했습니다.");
        }
        else if (card.cardType == CardType.Skill)
        {
            curHp = Mathf.Min(curHp + 10, battleStat.maxHP); // 체력 회복
            Debug.Log($"[카드 효과] 플레이어의 체력이 10 회복되었습니다.");
        }
    }

    public void OnDamage(float dmg)
    {
        float effectiveDmg = dmg;

        if (Armor > 0)
        {
            effectiveDmg -= Armor;
            Armor = Mathf.Max(0, Armor - dmg);
        }

        curHp -= effectiveDmg;

        if (curHp <= 0.0f)
        {
            myAnim.SetTrigger(animData.OnDead);
            Debug.Log("플레이어가 사망했습니다!");
            OnDead();
        }
        else
        {
            myAnim.SetTrigger(animData.OnDamage);
            Debug.Log($"플레이어가 {effectiveDmg}의 데미지를 받았습니다. 현재 체력: {curHp}");
        }
    }
}

