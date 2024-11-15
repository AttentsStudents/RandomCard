using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : BattleSystem
{
    public Transform targetMonster;
    private NavMeshAgent navMeshAgent;
    public float attackRange = 2.0f;
    private bool isTriggered = false;

    void Start()
    {
        OnReset();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent�� �ʿ��մϴ�.");
        }
    }

    void Update()
    {
        if (isTriggered && targetMonster != null)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            myAnim.SetFloat("x", x);
            myAnim.SetFloat("y", y);
            MoveToTargetAndAttack();
        }
    }

    public void TriggerAttack()
    {
        isTriggered = true;
    }

    private void MoveToTargetAndAttack() //�����Ϸ� ����
    {
        navMeshAgent.SetDestination(targetMonster.position);

        float distanceToTarget = Vector3.Distance(transform.position, targetMonster.position);
        if (distanceToTarget <= attackRange)
        {
            navMeshAgent.isStopped = true;
            OnAttack();
            isTriggered = false;
        }
        else
        {
            navMeshAgent.isStopped = false;
        }
    }

    public override void OnAttack() //�����Ҷ�
    {
        base.OnAttack();
        myAnim.SetTrigger(animData.OnAttack);

        if (targetMonster.TryGetComponent<IDamage>(out IDamage monsterDamage))
        {
            monsterDamage.OnDamage(battleStat.Attak);
        }
    }

    public new void OnDamage(float dmg) // ������ ������
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
            OnDead();
        }
        else
        {
            myAnim.SetTrigger(animData.OnDamage);
        }
    }

    public void MyTurn()
    {
        myAnim.SetBool(animData.MyTurn, true);
        myAnim.SetTrigger(animData.OnAttack);

        // �ִϸ��̼� ���� �� ���¸� ��Ȱ��ȭ
        StartCoroutine(ResetMyTurnAfterAnimation());
    }

    private IEnumerator ResetMyTurnAfterAnimation()
    {
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length); // ���� ������ �ִϸ��̼� ���̸�ŭ ���
        myAnim.SetBool(animData.MyTurn, false);
    }
}
