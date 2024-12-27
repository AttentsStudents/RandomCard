using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMap;

namespace Combat
{
    public abstract class Card : ImageProperty
    {
        public uint useCost = 1;
        public ItemCard data;
        public virtual bool Active(List<BattleSystem> battles)
        {
            if (useCost > GameData.playerStat.cost) return false;
            BattleManager.inst.player.anim.SetTrigger(AnimParams.OnAttack);
            GameData.playerStat.cost -= useCost;
            Effect(battles);

            BattleManager.inst.cost.OnUpdate();
            gameObject.transform.localPosition = Vector3.zero;
            CardPool.inst?.Push(gameObject);
            return true;
        }

        public abstract void Effect(List<BattleSystem> battles);
    }
}

