using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BattleStat
{
    public float maxHP { get; set; }
    public float curHP { get; set; }
    public float attack { get; set; }
    public float armor { get; set; }
    public uint maxCost { get; set; }
    public uint cost { get; set; }

    public BattleStat(float maxHP, float armor, float attack, uint cost = 0)
    {
        this.maxHP = maxHP;
        this.curHP = maxHP;
        this.armor = armor;
        this.attack = attack;
        this.maxCost = cost;
        this.cost = cost;
    }
}

public abstract class BattleSystem : MonoBehaviour, IBattleObserve, IDeathAlarm
{
    public UnityAction HpObserve { get; set; }
    public BattleStat battleStat { get; set; }
    public UnityAction DeathAlarm { get; set; }

    public void OnDamage(float damage)
    {
        HpChange(-damage);
        if (Mathf.Approximately(battleStat.curHP, 0.0f))
        {
            DeathAlarm?.Invoke();
            Destroy(gameObject);
        }
    }
    public void OnRecovery(float recovery)
    {
        HpChange(recovery);
    }

    void HpChange(float value)
    {
        battleStat.curHP = Mathf.Clamp(battleStat.curHP + value, 0.0f, battleStat.maxHP);
        HpObserve?.Invoke();
    }
}
