using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    public void AttackEffect(BattleSystem attacker, BattleSystem target)
    {
        float damage = attacker.battleStat.Attak;
        target.OnDamage(damage);
        Debug.Log($"[효과 발동] {attacker.gameObject.name}이(가) {target.gameObject.name}에게 {damage}의 피해를 입혔습니다!");
    }

    public void DefenseEffect(BattleSystem defender, BattleSystem _)
    {
        float defenseValue = 5;
        defender.Armor += defenseValue;
        Debug.Log($"[효과 발동] {defender.gameObject.name}이(가) 방어를 {defenseValue} 증가시켰습니다!");
    }

    public void HealEffect(BattleSystem healer, BattleSystem _)
    {
        float healValue = 10;
        healer.curHp += healValue;
        Debug.Log($"[효과 발동] {healer.gameObject.name}이(가) 체력을 {healValue} 회복했습니다!");
    }
}
