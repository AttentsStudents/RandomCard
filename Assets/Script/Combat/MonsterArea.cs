using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class MonsterArea : MonoBehaviour
    {
        public Transform[] areas;
        void Awake()
        {
            for (int i = 0; i < GameData.targetNode.monsterInfo.Count; i++)
            {
                var info = GameData.targetNode.monsterInfo[i];
                GameObject obj = Instantiate(MonsterGen.inst.list[info.Item1], areas[i]);
                if (obj.TryGetComponent<Monster>(out Monster monster))
                {
                    monster.level = info.Item2;
                    monster.isBattle = true;
                }
            }
        }
    }
}