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


    public struct BattleStat
    {
        public float maxHP;
        public float Cost;
        public float Armor;
        public float Attak;
        public float curHP;

    public BattleStat(float maxHP, float armor, float attack) : this() // 몬스터 배틀 스텟
    {
        this.maxHP = maxHP;
        Armor = armor;
        Attack = attack;
    }

    public float Attack { get; }
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

        public float curHp
        {
            get => battleStat.curHP;
            set
            {
                battleStat.curHP = value;
                hpObserbs?.Invoke(battleStat.curHP / battleStat.maxHP);
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
                Armor= 0.0f;
                curHp -= dmg-Armor;
            }
            else{
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

        public void OnAttack()
        {

            Target.GetComponent<IDamage>().OnDamage(battleStat.Attak);
        }

    
}
