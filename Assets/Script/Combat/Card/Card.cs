using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public abstract class Card : MonoBehaviour
    {
        public uint useCost = 1;
        public bool Active(List<BattleSystem> battles)
        {
            if (useCost > GameData.playerStat.cost) return false;
            Player.inst?.anim.SetTrigger(AnimParams.OnAttack);
            GameData.playerStat.cost -= useCost;
            TurnManager.inst?.PlayerTurnEvent?.Invoke();
            Effect(battles);
            return true;
        }

        public abstract void Effect(List<BattleSystem> battles);
    }
}

