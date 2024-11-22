using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class BossIsland : MiddleIsland
    {
        void Start()
        {
            sprite = SpriteManager.instance.icon.boss;
            ViewOnTheMap();
        }
    }
}

