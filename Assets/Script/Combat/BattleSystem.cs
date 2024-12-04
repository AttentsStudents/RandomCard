using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IDeathAlarm
{
    UnityAction deathAlarm { get; set; }
}
interface ILive
{
    bool IsLive { get; }
}

interface IDamage
{
    void OnDamage(float dmg);
}


[Serializable]
public class BattleStat
{
    public float maxHP;
    public float curHP;
    public float Armor;
    public float Attack;

    public BattleStat(float maxHP, float armor, float attack) // 몬스터 배틀 스텟
    {
        this.maxHP = maxHP;
        Armor = armor;
        Attack = attack;
    }
}

public class BattleSystem : AnimProperty
{
    public BattleStat battleStat;
    protected float playTime = 0.0f;
    public GameObject Target;
    public UnityAction deathAlarm { get; set; }
    public UnityEvent<float> hpObserbs;

    public bool IsLive
    {
        get => battleStat.curHP > 0.0f;
    }
    public float maxHp
    {
        get => battleStat.maxHP;
    }

    private bool isUpdatingHp = false;
    public float curHp
    {
        get => battleStat.curHP;
        set
        {
            if (isUpdatingHp) return;

            isUpdatingHp = true;
            float newHp = Mathf.Clamp(value, 0, battleStat.maxHP);

            if (!Mathf.Approximately(battleStat.curHP, newHp)) // 변경이 있을 때만 호출
            {
                battleStat.curHP = newHp;
                hpObserbs?.Invoke(battleStat.curHP / battleStat.maxHP);
            }

            isUpdatingHp = false;
        }
    }


    public float Armor
    {
        get => battleStat.Armor;
        set => battleStat.Armor = value;

    }

    protected void OnReset()
    {
        battleStat.Armor = 0.0f;
        battleStat.curHP = battleStat.maxHP;
    }

    protected virtual void OnDead()
    {
        deathAlarm?.Invoke();
    }

    public void OnDamage(float dmg)
    {
        curHp -= dmg;
        if (Armor > 0)
        {
            Armor -= dmg;
        }
        else if (dmg > Armor)
        {
            Armor = 0.0f;
            curHp -= dmg - Armor;
        }
        else
        {
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
    }

    public virtual void OnAttack()
    {
        if (Target == null)
        {
            Debug.LogError("OnAttack 호출 시 Target이 null입니다!");
            return;
        }

        if (Target.TryGetComponent<IDamage>(out IDamage damageable))
        {
            damageable.OnDamage(battleStat.Attack);
        }
        else
        {
            Debug.LogError($"{Target.name}에는 IDamage 컴포넌트가 없습니다!");
        }
    }


}
