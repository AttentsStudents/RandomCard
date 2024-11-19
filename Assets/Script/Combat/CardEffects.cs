using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    public void AttackEffect(BattleSystem attacker, BattleSystem target)
    {
        float damage = attacker.battleStat.Attak;
        target.OnDamage(damage);
        Debug.Log($"[ȿ�� �ߵ�] {attacker.gameObject.name}��(��) {target.gameObject.name}���� {damage}�� ���ظ� �������ϴ�!");
    }

    public void DefenseEffect(BattleSystem defender, BattleSystem _)
    {
        float defenseValue = 5;
        defender.Armor += defenseValue;
        Debug.Log($"[ȿ�� �ߵ�] {defender.gameObject.name}��(��) �� {defenseValue} �������׽��ϴ�!");
    }

    public void HealEffect(BattleSystem healer, BattleSystem _)
    {
        float healValue = 10;
        healer.curHp += healValue;
        Debug.Log($"[ȿ�� �ߵ�] {healer.gameObject.name}��(��) ü���� {healValue} ȸ���߽��ϴ�!");
    }
}
