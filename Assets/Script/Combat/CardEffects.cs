using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects : MonoBehaviour
{

    public interface ICardEffect
    {
        void ApplyEffect(BattleSystem player, List<Monster> monsters);
    }

    public class AttackEffect : ICardEffect
    {
        public void ApplyEffect(BattleSystem player, List<Monster> monsters)
        {
            if (monsters.Count > 0)
            {
                Monster target = monsters[UnityEngine.Random.Range(0, monsters.Count)];
                float damage = player.battleStat.Attak;
                target.OnDamage(damage);
                Debug.Log($"{player.gameObject.name}이(가) {target.gameObject.name}에게 {damage}의 피해를 입혔습니다!");
            }
        }
    }

    public class DefenseEffect : ICardEffect
    {
        public void ApplyEffect(BattleSystem player, List<Monster> _)
        {
            float defenseValue = 5;
            player.Armor += defenseValue;
            Debug.Log($"{player.gameObject.name}이(가) 방어를 {defenseValue} 증가시켰습니다!");
        }
    }

    public class HealEffect : ICardEffect
    {
        public void ApplyEffect(BattleSystem player, List<Monster> _)
        {
            float healValue = 10;
            player.curHp += healValue;
            Debug.Log($"{player.gameObject.name}이(가) 체력을 {healValue} 회복했습니다!");
        }
    }


    public class HealthSacrificeAttack : ICardEffect
    {
        private float healthCost;
        private float attackDamage;

        public HealthSacrificeAttack(float healthCost, float attackDamage)
        {
            this.healthCost = healthCost;
            this.attackDamage = attackDamage;
        }

        public void ApplyEffect(BattleSystem player, List<Monster> monsters)
        {
            player.curHp -= healthCost;
            Debug.Log($"플레이어가 체력을 {healthCost} 소모했습니다.");

            if (monsters.Count > 0)
            {
                Monster target = monsters[UnityEngine.Random.Range(0, monsters.Count)];
                target.OnDamage(attackDamage);
                Debug.Log($"{target.name}에게 {attackDamage}의 피해를 입혔습니다.");
            }
        }
    }
    public class StatusEffect : ICardEffect
    {
        private string statusName;
        private float duration;

        public StatusEffect(string statusName, float duration)
        {
            this.statusName = statusName;
            this.duration = duration;
        }

        public void ApplyEffect(BattleSystem player, List<Monster> monsters)
        {
            if (monsters.Count > 0)
            {
                Monster target = monsters[UnityEngine.Random.Range(0, monsters.Count)];
                target.ApplyStatusEffect(statusName, duration);
                Debug.Log($"{target.name}에게 상태 이상 '{statusName}'를 {duration}초 동안 부여했습니다.");
            }
        }
    }

}
