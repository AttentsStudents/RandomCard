using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Player : VisibleHpBar
    {
        public override BattleStat battleStat { get => GameData.playerStat; set => GameData.playerStat = value; }
        void Awake()
        {
            battleStat = new BattleStat(60, 0, 10);
            AddHpBar();
        }
    }
}

