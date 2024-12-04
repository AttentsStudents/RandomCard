using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BattleSystem, IDamage
{
    public int monsterId;
    public int level;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdleAnimation()
    {
        animator.SetBool("IsMoving", false);
    }

    public void PlayMoveAnimation()
    {
        animator.SetBool("IsMoving", true);
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("OnAttack");
    }

    public void PlayDamageAnimation()
    {
        animator.SetTrigger("OnDamage");
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("OnDead");
    }

    public void OnDamage(float damage)
    {
        curHp -= damage;

        if (curHp <= 0)
        {
            PlayDeathAnimation();
            Debug.Log($"{name}��(��) ����߽��ϴ�.");
            OnDead();
        }
        else
        {
            PlayDamageAnimation();
            Debug.Log($"{name}��(��) {damage}�� �������� �޾ҽ��ϴ�.");
        }
    }


    public override void OnAttack()
    {
        base.OnAttack();
        PlayAttackAnimation();
        Debug.Log($"[OnAttack] {name}��(��) ������ �����մϴ�.");

        if (Target.TryGetComponent<IDamage>(out IDamage damageable))
        {
            float attackDamage = battleStat.Attack;
            Debug.Log($"[OnAttack] {Target.name}���� {attackDamage}�� ���ظ� �����ϴ�.");
            damageable.OnDamage(attackDamage); // Ÿ�ٿ� ������ ����
        }
        else
        {
            Debug.LogError($"[OnAttack] {Target.name}���� IDamage �������̽��� �������� �ʾҽ��ϴ�!");
        }
    }



    public void Initialize(int id, int lv, MonsterData data, GameObject target)
    {
        monsterId = id;
        level = lv;
        float maxHP = data.maxHP + (level * 5); // ������ ���� ü�� ����
        float armor = data.armor;
        float attack = data.attack + (level * 2); // ������ ���� ���ݷ� ����

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP;

        Target = target;
        transform.LookAt(Target.transform); // Ÿ�� �ٶ󺸱�
    }


    public void DisplayStats()
    {
        Debug.Log($"Monster ID: {monsterId}, Level: {level}, HP: {battleStat.curHP}, Armor: {battleStat.Armor}, Attack: {battleStat.Attack}");
    }
        private Dictionary<string, float> statusEffects = new Dictionary<string, float>();

        public void ApplyStatusEffect(string statusName, float duration)
        {
            if (statusEffects.ContainsKey(statusName))
            {
                statusEffects[statusName] = duration; // ���� ���� ����
            }
            else
            {
                statusEffects.Add(statusName, duration);
                StartCoroutine(HandleStatusEffect(statusName, duration));
            }
        }

    private IEnumerator HandleStatusEffect(string statusName, float duration)
    {
        Debug.Log($"{name}��(��) ���� �̻� '{statusName}'�� �ɷȽ��ϴ�.");
        while (duration > 0)
        {
            switch (statusName)
            {
                case "Poison":
                    OnDamage(5); // ���� 5 ������
                    break;

                case "Burn":
                    OnDamage(10); // ���� 10 ������
                    break;

                case "Freeze":
                    Debug.Log($"{name}��(��) ���پ� �ൿ�� �� �����ϴ�.");
                    yield break; // �ൿ ����
            }

            duration -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }

        statusEffects.Remove(statusName);
        Debug.Log($"{name}�� ���� �̻� '{statusName}'�� ����Ǿ����ϴ�.");
    }

}