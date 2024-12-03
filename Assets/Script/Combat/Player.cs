using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : BattleSystem, IDamage
{
    public Transform targetMonster;
    private NavMeshAgent navMeshAgent;
    public Animator anim;
    public float attackRange = 2.0f; // Ÿ�ٰ��� �ּ� �Ÿ�
    public PlayerData playerData;

    private Vector3 originalPosition; // ���� ��ġ
    private bool isMovingToTarget = false; // �̵� ������ Ȯ��

    void Start()
    {
        OnReset();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent�� �ʿ��մϴ�.");
        }
        InitializePlayerStats();
    }

    private void InitializePlayerStats()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData�� ������� �ʾҽ��ϴ�.");
            return;
        }
        battleStat = new BattleStat(playerData.maxHP, playerData.armor, playerData.attack);
        curHp = battleStat.maxHP; // ���� ü���� �ִ� ü������ ����
    }

    public void PlayCardsSequentially(Card[] cards, Monster[] targetMonsters, System.Action onAllEffectsComplete)
    {
        originalPosition = transform.position; // �÷��̾� �ʱ� ��ġ ����
        StartCoroutine(ExecuteCardsSequentially(cards, targetMonsters, onAllEffectsComplete));
    }

    private IEnumerator ExecuteCardsSequentially(Card[] cards, Monster[] targetMonsters, System.Action onAllEffectsComplete)
    {
        if (cards == null || targetMonsters == null || cards.Length == 0)
        {
            Debug.LogError("ī�� �迭 �Ǵ� Ÿ�� ���� �迭�� ��� �ֽ��ϴ�.");
            onAllEffectsComplete?.Invoke();
            yield break;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            Monster targetMonster = targetMonsters[i % targetMonsters.Length];
            Debug.Log($"ī�� {i + 1}/{cards.Length} ���� ��, Ÿ��: {targetMonster.name}");

            // Ÿ�� ���� ������ �̵�
            yield return MoveToTargetAndExecuteAction(targetMonster.transform, () =>
            {
                Debug.Log($"ī�� {i + 1}: Ÿ�� {targetMonster.name} ��ó ����");
            });

            // ī�� ȿ�� ����
            bool effectApplied = false;
            StartCoroutine(PlayAnimationAndEffect(cards[i], targetMonster, () =>
            {
                Debug.Log($"ī�� {i + 1}: ȿ�� ���� �Ϸ�");
                effectApplied = true;
            }));

            // ȿ�� ���� �Ϸ� ���
            while (!effectApplied)
            {
                yield return null;
            }

            // ���� ��ġ�� ����
            Debug.Log($"ī�� {i + 1}: ȿ�� �Ϸ� �� ���� ��ġ�� ���� ��");
            yield return ReturnToOriginalPosition(() =>
            {
                Debug.Log($"ī�� {i + 1}: ���� �Ϸ�");
            });
        }

        // ��� ī�� ȿ�� ���� �Ϸ�
        Debug.Log("��� ī�� ȿ�� ���� �Ϸ�");
        onAllEffectsComplete?.Invoke();
    }


    private IEnumerator PlayAnimationAndEffect(Card card, Monster targetMonster, System.Action onEffectComplete)
    {
        // ī�� Ÿ�Կ� ���� �ִϸ��̼� ����
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

        // �ִϸ��̼� ���� Ȯ�� �� ���
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);
        Debug.Log($"�ִϸ��̼� ����: {card.cardType}");

        ApplyCardEffect(card, targetMonster);

        Debug.Log("ī�� ȿ�� ���� �Ϸ�");
        onEffectComplete?.Invoke();
    }


    private IEnumerator MoveToTargetAndExecuteAction(Transform targetTransform, System.Action onArrived)
    {
        if (navMeshAgent == null || targetTransform == null)
        {
            Debug.LogError("NavMeshAgent �Ǵ� Ÿ�� Transform�� null�Դϴ�.");
            yield break;
        }

        navMeshAgent.SetDestination(targetTransform.position);

        // Ÿ�� ��ġ�� ������ ������ ���
        while (Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        onArrived?.Invoke(); // Ÿ�� ��ó ���� �ݹ� ����
    }


    private IEnumerator ReturnToOriginalPosition(System.Action onReturned)
    {
        if (originalPosition == null)
        {
            Debug.LogError("���� ��ġ ������ null�Դϴ�.");
            yield break;
        }

        navMeshAgent.SetDestination(originalPosition);

        // ���� ��ġ�� �̵�
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        onReturned?.Invoke(); // ���� �Ϸ� �ݹ� ����
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
            targetMonster.OnDamage(battleStat.Attak * 10); // ���ݷ� ��� ������ ����
            Debug.Log($"[ī�� ȿ��] {targetMonster.name}���� {battleStat.Attak * 10}�� ���ظ� �������ϴ�.");
        }
        else if (card.cardType == CardType.Defense)
        {
            Armor += 5; // ���� ����
            Debug.Log($"[ī�� ȿ��] �÷��̾��� ������ 5 �����߽��ϴ�.");
        }
        else if (card.cardType == CardType.Skill)
        {
            curHp = Mathf.Min(curHp + 10, battleStat.maxHP); // ü�� ȸ��
            Debug.Log($"[ī�� ȿ��] �÷��̾��� ü���� 10 ȸ���Ǿ����ϴ�.");
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
            Debug.Log("�÷��̾ ����߽��ϴ�!");
            OnDead();
        }
        else
        {
            myAnim.SetTrigger(animData.OnDamage);
            Debug.Log($"�÷��̾ {effectiveDmg}�� �������� �޾ҽ��ϴ�. ���� ü��: {curHp}");
        }
    }
}

