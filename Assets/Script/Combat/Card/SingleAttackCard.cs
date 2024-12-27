using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class SingleAttackCard : DragCard
    {
        public float attack = 15.0f;
        public float decreseHp = 0.0f;

        public override void Effect(List<BattleSystem> battles)
        {
            foreach (BattleSystem battle in battles)
            {
                battle.OnDamage(attack);
                if (decreseHp > 0.0f) BattleManager.inst.player.OnDamageNoMotion(decreseHp);
            }
        }
    }
}

