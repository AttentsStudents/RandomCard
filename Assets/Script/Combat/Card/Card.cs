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
            if(BattleManager.inst.turn != BattleManager.Turn.PLAYER)
            {
                AlertMessage.Alert("플레이어의 턴이 아닙니다");
                return false;
            }

            if (useCost > GameData.playerStat.cost)
            {
                AlertMessage.Alert("코스트가 부족합니다");
                return false;
            }
            BattleManager.inst.player.anim.SetTrigger(AnimParams.OnAttack);
            GameData.playerStat.cost -= useCost;
            Effect(battles);

            BattleManager.inst.cost.OnUpdate();
            gameObject.transform.localPosition = Vector3.zero;
            CardPool.inst?.Push(gameObject);
            BattleManager.inst.OnCheckCardsInHand();
            return true;
        }

        public abstract void Effect(List<BattleSystem> battles);
    }
}

