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
    private bool isTriggered;

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
    void Update()
    {
        // NavMeshAgent �̵� ������ Animator�� ����
        if (navMeshAgent != null)
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            myAnim.SetFloat("X", localVelocity.x);
            myAnim.SetFloat("Y", localVelocity.z);

            bool isMoving = velocity.magnitude > 0.1f;
            myAnim.SetBool(animData.IsMove, isMoving);
        }

        // ��Ʈ ����� Ȱ���Ͽ� �̵�
        if (isTriggered && targetMonster != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetMonster.position);

            if (distanceToTarget > attackRange)
            {
                navMeshAgent.isStopped = true; // NavMeshAgent ����
                Vector3 direction = (targetMonster.position - transform.position).normalized;
                transform.Translate(direction * Time.deltaTime, Space.World); // ��Ʈ ��� ��� �̵�
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
            Debug.LogError("PlayerData�� ������� �ʾҽ��ϴ�.");
            return;
        }
        battleStat = new BattleStat(playerData.maxHP, playerData.armor, playerData.attack);
        curHp = battleStat.maxHP; // ���� ü���� �ִ� ü������ ����
    }

    private IEnumerator RotateTowards(Transform targetTransform, float duration)
    {
        Debug.Log("�÷��̾ ���͸� �ٶ󺸴� ��...");

        Quaternion initialRotation = transform.rotation; // ���� ȸ�� ����
        Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - transform.position); // ��ǥ ȸ�� ����

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ȸ�� ���� (Lerp �Ǵ� Slerp ��� ����)
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���������� ��Ȯ�� Ÿ�� ������ �ٶ󺸵��� ����
        transform.rotation = targetRotation;

        Debug.Log("�÷��̾ ���͸� �ٶ󺾴ϴ�.");
    }

    public void PlayCardsSequentially(Card[] cards, Monster targetMonster, System.Action onAllEffectsComplete)
    {
        originalPosition = transform.position; // �̵� �� ��ġ ����

        StartCoroutine(ExecuteCardsSequentially(cards, targetMonster, onAllEffectsComplete));
    }

    private IEnumerator ExecuteCardsSequentially(Card[] cards, Monster targetMonster, System.Action onAllEffectsComplete)
    {
        if (cards == null || cards.Length == 0 || targetMonster == null)
        {
            Debug.LogError("ī�� �迭 �Ǵ� Ÿ�� ���Ͱ� ��� �ֽ��ϴ�.");
            yield break;
        }

        // Ÿ�� ��ó�� �̵�
        yield return MoveToTarget(targetMonster.transform);

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            bool effectApplied = false;

            Debug.Log($"ī�� {i + 1}/{cards.Length} ȿ�� ����");

            // ī�� ȿ�� ����
            StartCoroutine(PlayAnimationAndEffect(cards[i], targetMonster, () =>
            {
                effectApplied = true; // ī�� ȿ�� �Ϸ� ��ȣ
            }));

            // ȿ���� ���� ������ ���
            while (!effectApplied)
            {
                yield return null;
            }
        }

        Debug.Log("��� ī�� ȿ�� ���� �Ϸ�. ���� ��ġ�� ���� ��...");

        // ���� ��ġ�� ����
        yield return ReturnToOriginalPosition(() =>
        {
            Debug.Log("���� ��ġ ���� �Ϸ�.");
        });

        // ���� �� ���� ���� �ٶ󺸱�
        yield return RotateTowards(targetMonster.transform, 1.0f); // ȸ�� ���� �ð� 1�� ����

        onAllEffectsComplete?.Invoke(); // ��ü ȿ�� �Ϸ� ��ȣ
    }

    private IEnumerator MoveToTarget(Transform targetTransform)
    {
        Debug.Log($"Ÿ�� {targetTransform.name} ��ó�� �̵��մϴ�.");

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent�� ������� �ʾҽ��ϴ�.");
            yield break;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetTransform.position);

        myAnim.SetBool(animData.IsMove, true); // �ȱ� �ִϸ��̼� Ȱ��ȭ

        while (Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            yield return null; // Ÿ�ٿ� ������ ������ ���
        }

        navMeshAgent.isStopped = true;
        myAnim.SetBool(animData.IsMove, false); // �ȱ� �ִϸ��̼� ��Ȱ��ȭ

        Debug.Log($"Ÿ�� {targetTransform.name} ��ó�� �����߽��ϴ�.");
    }

    private IEnumerator PlayAnimationAndEffect(Card card, Monster targetMonster, System.Action onEffectComplete)
    {
        Debug.Log($"�ִϸ��̼� ���� ����: {card.cardType}");

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

        // �ִϸ��̼� ���
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);

        Debug.Log("�ִϸ��̼� ����. ī�� ȿ�� ���� ��...");

        // ī�� ȿ�� ����
        ApplyCardEffect(card, targetMonster);

        Debug.Log($"ī�� {card.cardName} ȿ�� ���� �Ϸ�.");
        onEffectComplete?.Invoke();
    }

    private IEnumerator ReturnToOriginalPosition(System.Action onReturned)
    {
        Debug.Log("���� ��ġ�� ���ư��ϴ�.");

        if (originalPosition == Vector3.zero)
        {
            Debug.LogError("���� ��ġ ������ �������� �ʾҽ��ϴ�.");
            yield break;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(originalPosition);

        myAnim.SetBool(animData.IsMove, true); // �ȱ� �ִϸ��̼� Ȱ��ȭ

        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            yield return null; // ��ġ�� ������ ������ ���
        }

        navMeshAgent.isStopped = true;
        myAnim.SetBool(animData.IsMove, false); // �ȱ� �ִϸ��̼� ��Ȱ��ȭ

        Debug.Log("���� ��ġ�� �����߽��ϴ�.");
        onReturned?.Invoke(); // ���� �Ϸ� �ݹ� ����
    }

    private void ApplyCardEffect(Card card, Monster targetMonster)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                targetMonster.OnDamage(card.effectValue); // ī�� ������ ����
                Debug.Log($"{targetMonster.name}���� {card.effectValue}�� �������� �������ϴ�.");
                break;

            case CardType.Defense:
                Armor += card.effectValue; // ���� ����
                Debug.Log($"�÷��̾� ������ {card.effectValue}��ŭ �����߽��ϴ�.");
                break;

            case CardType.Skill:
                curHp = Mathf.Min(curHp + card.effectValue, maxHp); // ü�� ȸ��
                Debug.Log($"�÷��̾� ü���� {card.effectValue}��ŭ ȸ���Ǿ����ϴ�. ���� ü��: {curHp}");
                break;

            default:
                Debug.LogWarning($"�� �� ���� ī�� Ÿ��: {card.cardType}");
                break;
        }
    }

    public void OnDamage(float dmg)
    {
        Debug.Log($"[OnDamage ȣ���] �޴� ������: {dmg}"); // �߰��� ����� �α�

        float effectiveDmg = dmg;

        if (Armor > 0)
        {
            effectiveDmg -= Armor;
            Armor = Mathf.Max(0, Armor - dmg);
        }

        curHp -= effectiveDmg;

        Debug.Log($"[OnDamage] ����� ������: {effectiveDmg}, ���� ü��: {curHp}");

        if (curHp <= 0.0f)
        {
            myAnim.SetTrigger(animData.OnDead);
            Debug.Log("�÷��̾ ����߽��ϴ�!");
            OnDead();
        }
        else
        {
            myAnim.SetTrigger(animData.OnDamage);
            Debug.Log($"[OnDamage] �÷��̾ {effectiveDmg}�� �������� �޾ҽ��ϴ�. ���� ü��: {curHp}");
        }
    }

}

