using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class SingleAttackCard : DragCard
    {
        public float Attack = 15.0f;
        public override void Effect(List<BattleSystem> battles)
        {
            foreach (BattleSystem battle in battles)
            {
                battle.OnDamage(Attack);
            }
        }
    }
}

