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

    public void PlayCardEffectOnTarget(Card card, Monster targetMonster, System.Action onEffectComplete)
    {
        StartCoroutine(MoveToTargetAndExecuteAction(targetMonster.transform, () =>
        {
            // 카드 타입에 따른 애니메이션 실행
            PlayAnimation(card);

            // 카드 효과 실행
            ApplyCardEffect(card, targetMonster);

            // 효과 완료 후 콜백 실행
            onEffectComplete?.Invoke();
        }));
    }

    private IEnumerator MoveToTargetAndExecuteAction(Transform targetTransform, System.Action onArrived)
    {
        if (targetTransform == null)
        {
            Debug.LogError("타겟이 설정되지 않았습니다.");
            yield break;
        }

        isMovingToTarget = true;
        navMeshAgent.SetDestination(targetTransform.position);

        // 타겟에 도달할 때까지 대기
        while (Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        isMovingToTarget = false;

        // 도착 후 액션 실행
        onArrived?.Invoke();
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

