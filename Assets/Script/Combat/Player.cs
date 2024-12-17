using WorldMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Player : VisibleHpBar
    {
        void Awake()
        {
            battleStat = GameData.playerStat;
            AddHpBar();
        }
    }
}

