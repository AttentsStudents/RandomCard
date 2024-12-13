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
            Debug.Log($"{name}이(가) 사망했습니다.");
            OnDead();
        }
        else
        {
            PlayDamageAnimation();
            Debug.Log($"{name}이(가) {damage}의 데미지를 받았습니다.");
        }
    }


    public override void OnAttack()
    {
        base.OnAttack();
        PlayAttackAnimation();
        Debug.Log($"[OnAttack] {name}이(가) 공격을 실행합니다.");

        if (Target.TryGetComponent<IDamage>(out IDamage damageable))
        {
            float attackDamage = battleStat.Attack;
            Debug.Log($"[OnAttack] {Target.name}에게 {attackDamage}의 피해를 입힙니다.");
            damageable.OnDamage(attackDamage); // 타겟에 데미지 적용
        }
        else
        {
            Debug.LogError($"[OnAttack] {Target.name}에는 IDamage 인터페이스가 구현되지 않았습니다!");
        }
    }



    public void Initialize(int id, int lv, MonsterData data, GameObject target)
    {
        monsterId = id;
        level = lv;
        float maxHP = data.maxHP + (level * 5); // 레벨에 따른 체력 증가
        float armor = data.armor;
        float attack = data.attack + (level * 2); // 레벨에 따른 공격력 증가

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP;

        Target = target;
        transform.LookAt(Target.transform); // 타겟 바라보기
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
                statusEffects[statusName] = duration; // 기존 상태 갱신
            }
            else
            {
                statusEffects.Add(statusName, duration);
                StartCoroutine(HandleStatusEffect(statusName, duration));
            }
        }

    private IEnumerator HandleStatusEffect(string statusName, float duration)
    {
        Debug.Log($"{name}이(가) 상태 이상 '{statusName}'에 걸렸습니다.");
        while (duration > 0)
        {
            switch (statusName)
            {
                case "Poison":
                    OnDamage(5); // 매초 5 데미지
                    break;

                case "Burn":
                    OnDamage(10); // 매초 10 데미지
                    break;

                case "Freeze":
                    Debug.Log($"{name}이(가) 얼어붙어 행동할 수 없습니다.");
                    yield break; // 행동 제한
            }

            duration -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }

        statusEffects.Remove(statusName);
        Debug.Log($"{name}의 상태 이상 '{statusName}'이 종료되었습니다.");
    }

}