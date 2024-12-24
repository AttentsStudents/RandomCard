using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public class MultiAttackCard : ClickCard
    {
        public float attack = 10.0f;

        public override void Effect(List<BattleSystem> battles)
        {
            foreach (BattleSystem battle in battles)
            {
                battle.OnDamage(attack);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (MonsterArea.inst?.monsters != null) Active(MonsterArea.inst.monsters.ToList<BattleSystem>());
        }
    }
}

