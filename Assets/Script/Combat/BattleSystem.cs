using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BattleStat
{
    public float maxHP;
    public float curHP;
    public float attack;
    public float armor;

    public BattleStat(float hp, float armor, float attack)
    {
        maxHP = hp;
        curHP = hp;
        this.armor = armor;
        this.attack = attack;
    }
}

[Serializable]
public class PlayerStat : BattleStat
{
    public uint maxCost;
    public uint cost;
    public uint recoveryCost;
    public uint gold;
    public PlayerStat(float hp, float armor, float attack, uint cost) : base(hp,armor,attack)
    {
        maxCost = cost;
        this.cost = cost;
        recoveryCost = 3;
        gold = 0;
    }
}

public abstract class BattleSystem : AnimProperty, IBattleObserve, IDeathAlarm
{
    public UnityAction HpObserve { get; set; }
    public BattleStat battleStat { get; set; }
    public UnityAction DeathAlarm { get; set; }

    public void OnDamage(float damage)
    {
        HpChange(-damage);
        Instantiate(ObjectManager.inst.effect.hit, transform);

        if (Mathf.Approximately(battleStat.curHP, 0.0f))
        {
            DeathAlarm?.Invoke();
            anim.SetTrigger(AnimParams.OnDead);
        }
        else
        {
            anim.SetTrigger(AnimParams.OnDamage);
        }
    }
    public void OnRecovery(float recovery)
    {
        Instantiate(ObjectManager.inst.effect.heal, transform);
        HpChange(recovery);
    }

    public void OnBuff()
    {
        Instantiate(ObjectManager.inst.effect.buff, transform);
    }

    void HpChange(float value)
    {
        battleStat.curHP = Mathf.Clamp(battleStat.curHP + value, 0.0f, battleStat.maxHP);
        HpObserve?.Invoke();
    }
}
