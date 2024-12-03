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

    public void PlayCardEffectOnTarget(Card card, Monster targetMonster, System.Action onEffectComplete)
    {
        StartCoroutine(MoveToTargetAndExecuteAction(targetMonster.transform, () =>
        {
            // ī�� Ÿ�Կ� ���� �ִϸ��̼� ����
            PlayAnimation(card);

            // ī�� ȿ�� ����
            ApplyCardEffect(card, targetMonster);

            // ȿ�� �Ϸ� �� �ݹ� ����
            onEffectComplete?.Invoke();
        }));
    }

    private IEnumerator MoveToTargetAndExecuteAction(Transform targetTransform, System.Action onArrived)
    {
        if (targetTransform == null)
        {
            Debug.LogError("Ÿ���� �������� �ʾҽ��ϴ�.");
            yield break;
        }

        isMovingToTarget = true;
        navMeshAgent.SetDestination(targetTransform.position);

        // Ÿ�ٿ� ������ ������ ���
        while (Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            yield return null;
        }

        navMeshAgent.isStopped = true;
        isMovingToTarget = false;

        // ���� �� �׼� ����
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

