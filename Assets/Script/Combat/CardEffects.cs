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
                Debug.Log($"{player.gameObject.name}��(��) {target.gameObject.name}���� {damage}�� ���ظ� �������ϴ�!");
            }
        }
    }

    public class DefenseEffect : ICardEffect
    {
        public void ApplyEffect(BattleSystem player, List<Monster> _)
        {
            float defenseValue = 5;
            player.Armor += defenseValue;
            Debug.Log($"{player.gameObject.name}��(��) �� {defenseValue} �������׽��ϴ�!");
        }
    }

    public class HealEffect : ICardEffect
    {
        public void ApplyEffect(BattleSystem player, List<Monster> _)
        {
            float healValue = 10;
            player.curHp += healValue;
            Debug.Log($"{player.gameObject.name}��(��) ü���� {healValue} ȸ���߽��ϴ�!");
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
            Debug.Log($"�÷��̾ ü���� {healthCost} �Ҹ��߽��ϴ�.");

            if (monsters.Count > 0)
            {
                Monster target = monsters[UnityEngine.Random.Range(0, monsters.Count)];
                target.OnDamage(attackDamage);
                Debug.Log($"{target.name}���� {attackDamage}�� ���ظ� �������ϴ�.");
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
                Debug.Log($"{target.name}���� ���� �̻� '{statusName}'�� {duration}�� ���� �ο��߽��ϴ�.");
            }
        }
    }

}
