using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BattleStat
{
    public float maxHP { get; set; }
    public float curHP { get; set; }
    public float attack { get; set; }
    public float armor { get; set; }

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
    public uint maxCost { get; set; }
    public uint cost { get; set; }
    public PlayerStat(float hp, float armor, float attack, uint cost) : base(hp,armor,attack)
    {
        maxCost = cost;
        this.cost = cost;
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

    void HpChange(float value)
    {
        battleStat.curHP = Mathf.Clamp(battleStat.curHP + value, 0.0f, battleStat.maxHP);
        HpObserve?.Invoke();
    }
}
