using WorldMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Player : VisibleHpBar
    {
        public static Player inst { get; private set; }
        void Awake()
        {
            inst = this;
            battleStat = GameData.playerStat;
            AddHpBar();
        }
    }
}

