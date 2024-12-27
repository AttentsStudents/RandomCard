using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public class ShieldCard : ClickCard
    {
        public float addArmor = 10.0f;
        public override void Effect(List<BattleSystem> battles)
        {
            foreach (BattleSystem battle in battles)
            {
                battle.battleStat.armor += addArmor;
                battle.OnBuff();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            Active(new List<BattleSystem>() { BattleManager.inst.player });
            BattleManager.inst.armor.OnUpdate();
        }
    }
}

