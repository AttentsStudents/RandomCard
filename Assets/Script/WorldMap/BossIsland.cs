using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CheonJiWoon.ObjectManager;

namespace CheonJiWoon
{
    public class BossIsland : MiddleIsland
    {
        void Start()
        {
            sprite = SpriteManager.instance.icon.boss;
            ViewOnTheMap();

            myNode.monsterInfo = new List<(int, int)>();
            RandomMonster(100, 100);
            CreateMonster(myNode.monsterInfo[0].Item1, center);

            CrashAction = () =>
            {
                GameData.enemies = myNode.monsterInfo;
                Loading.LoadScene(Scene.BATTLE);
            };
        }
    }
}

