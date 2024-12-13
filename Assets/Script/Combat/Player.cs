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
    private bool isTriggered;

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
    void Update()
    {
        // NavMeshAgent 이동 방향을 Animator에 전달
        if (navMeshAgent != null)
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            myAnim.SetFloat("X", localVelocity.x);
            myAnim.SetFloat("Y", localVelocity.z);

            bool isMoving = velocity.magnitude > 0.1f;
            myAnim.SetBool(animData.IsMove, isMoving);
        }

        // 루트 모션을 활용하여 이동
        if (isTriggered && targetMonster != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetMonster.position);

            if (distanceToTarget > attackRange)
            {
                navMeshAgent.isStopped = true; // NavMeshAgent 멈춤
                Vector3 direction = (targetMonster.position - transform.position).normalized;
                transform.Translate(direction * Time.deltaTime, Space.World); // 루트 모션 기반 이동
            }
            else
            {
                isTriggered = false;
                OnAttack();
            }
        }
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

    private IEnumerator RotateTowards(Transform targetTransform, float duration)
    {
        Debug.Log("플레이어가 몬스터를 바라보는 중...");

        Quaternion initialRotation = transform.rotation; // 현재 회전 상태
        Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - transform.position); // 목표 회전 상태

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 회전 보간 (Lerp 또는 Slerp 사용 가능)
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종적으로 정확히 타겟 방향을 바라보도록 설정
        transform.rotation = targetRotation;

        Debug.Log("플레이어가 몬스터를 바라봅니다.");
    }

    public void PlayCardsSequentially(Card[] cards, Monster targetMonster, System.Action onAllEffectsComplete)
    {
        originalPosition = transform.position; // 이동 전 위치 저장

        StartCoroutine(ExecuteCardsSequentially(cards, targetMonster, onAllEffectsComplete));
    }

    private IEnumerator ExecuteCardsSequentially(Card[] cards, Monster targetMonster, System.Action onAllEffectsComplete)
    {
        if (cards == null || cards.Length == 0 || targetMonster == null)
        {
            Debug.LogError("카드 배열 또는 타겟 몬스터가 비어 있습니다.");
            yield break;
        }

        // 타겟 근처로 이동
        yield return MoveToTarget(targetMonster.transform);

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            bool effectApplied = false;

            Debug.Log($"카드 {i + 1}/{cards.Length} 효과 실행");

            // 카드 효과 실행
            StartCoroutine(PlayAnimationAndEffect(cards[i], targetMonster, () =>
            {
                effectApplied = true; // 카드 효과 완료 신호
            }));

            // 효과가 끝날 때까지 대기
            while (!effectApplied)
            {
                yield return null;
            }
        }

        Debug.Log("모든 카드 효과 실행 완료. 원래 위치로 복귀 중...");

        // 원래 위치로 복귀
        yield return ReturnToOriginalPosition(() =>
        {
            Debug.Log("원래 위치 복귀 완료.");
        });

        // 복귀 후 몬스터 방향 바라보기
        yield return RotateTowards(targetMonster.transform, 1.0f); // 회전 지속 시간 1초 설정

        onAllEffectsComplete?.Invoke(); // 전체 효과 완료 신호
    }

    private IEnumerator MoveToTarget(Transform targetTransform)
    {
        Debug.Log($"타겟 {targetTransform.name} 근처로 이동합니다.");

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent가 연결되지 않았습니다.");
            yield break;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetTransform.position);

        myAnim.SetBool(animData.IsMove, true); // 걷기 애니메이션 활성화

        while (Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            yield return null; // 타겟에 도달할 때까지 대기
        }

        navMeshAgent.isStopped = true;
        myAnim.SetBool(animData.IsMove, false); // 걷기 애니메이션 비활성화

        Debug.Log($"타겟 {targetTransform.name} 근처에 도착했습니다.");
    }

    private IEnumerator PlayAnimationAndEffect(Card card, Monster targetMonster, System.Action onEffectComplete)
    {
        Debug.Log($"애니메이션 실행 시작: {card.cardType}");

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

        // 애니메이션 대기
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);

        Debug.Log("애니메이션 종료. 카드 효과 적용 중...");

        // 카드 효과 적용
        ApplyCardEffect(card, targetMonster);

        Debug.Log($"카드 {card.cardName} 효과 적용 완료.");
        onEffectComplete?.Invoke();
    }

    private IEnumerator ReturnToOriginalPosition(System.Action onReturned)
    {
        Debug.Log("원래 위치로 돌아갑니다.");

        if (originalPosition == Vector3.zero)
        {
            Debug.LogError("원래 위치 정보가 설정되지 않았습니다.");
            yield break;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(originalPosition);

        myAnim.SetBool(animData.IsMove, true); // 걷기 애니메이션 활성화

        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            yield return null; // 위치에 도달할 때까지 대기
        }

        navMeshAgent.isStopped = true;
        myAnim.SetBool(animData.IsMove, false); // 걷기 애니메이션 비활성화

        Debug.Log("원래 위치에 도달했습니다.");
        onReturned?.Invoke(); // 복귀 완료 콜백 실행
    }

    private void ApplyCardEffect(Card card, Monster targetMonster)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                targetMonster.OnDamage(card.effectValue); // 카드 데미지 적용
                Debug.Log($"{targetMonster.name}에게 {card.effectValue}의 데미지를 입혔습니다.");
                break;

            case CardType.Defense:
                Armor += card.effectValue; // 방어력 증가
                Debug.Log($"플레이어 방어력이 {card.effectValue}만큼 증가했습니다.");
                break;

            case CardType.Skill:
                curHp = Mathf.Min(curHp + card.effectValue, maxHp); // 체력 회복
                Debug.Log($"플레이어 체력이 {card.effectValue}만큼 회복되었습니다. 현재 체력: {curHp}");
                break;

            default:
                Debug.LogWarning($"알 수 없는 카드 타입: {card.cardType}");
                break;
        }
    }

    public void OnDamage(float dmg)
    {
        Debug.Log($"[OnDamage 호출됨] 받는 데미지: {dmg}"); // 추가된 디버그 로그

        float effectiveDmg = dmg;

        if (Armor > 0)
        {
            effectiveDmg -= Armor;
            Armor = Mathf.Max(0, Armor - dmg);
        }

        curHp -= effectiveDmg;

        Debug.Log($"[OnDamage] 적용된 데미지: {effectiveDmg}, 남은 체력: {curHp}");

        if (curHp <= 0.0f)
        {
            myAnim.SetTrigger(animData.OnDead);
            Debug.Log("플레이어가 사망했습니다!");
            OnDead();
        }
        else
        {
            myAnim.SetTrigger(animData.OnDamage);
            Debug.Log($"[OnDamage] 플레이어가 {effectiveDmg}의 데미지를 받았습니다. 현재 체력: {curHp}");
        }
    }

}

