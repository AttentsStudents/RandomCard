using WorldMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Player : VisibleHpBar
    {
        public UnityEvent DeathEvent;

        void Awake()
        {
            BattleManager.inst.player = this;
            battleStat = GameData.playerStat;
            battleStat.armor = 0;
            AddHpBar();
            DeathAlarm += () => DeathEvent?.Invoke();
        }
    }
}

