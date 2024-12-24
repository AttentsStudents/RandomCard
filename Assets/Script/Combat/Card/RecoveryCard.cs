using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public class RecoveryCard : ClickCard
    {
        public float hpRecovery = 10.0f;
        public override void Effect(List<BattleSystem> battles)
        {
            foreach (BattleSystem battle in battles)
            {
                battle.OnRecovery(hpRecovery);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if(Player.inst != null) Active(new List<BattleSystem>() { { Player.inst } });
        }
    }
}
