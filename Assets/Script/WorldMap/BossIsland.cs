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

            if (myNode.monsterInfo == null || myNode.monsterInfo.Count == 0)
            {
                myNode.monsterInfo = new List<(int, int)>();
                RandomMonster(100, 100);
            }
            CreateMonster(myNode.monsterInfo[0].Item1, center);

            CrashAction = () =>
            {
                GameData.targetNode = myNode;
                Loading.LoadScene(Scene.BATTLE);
            };
        }
    }
}

