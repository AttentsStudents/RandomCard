using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class MonsterArea : MonoBehaviour
    {
        public Transform[] areas;
        public UnityEvent MonsterWipeoutEvent;
        public UnityEvent MonsterActionEndEvent;
        public HashSet<Monster> monsters { get; private set; }
        int monsterActionCount = 0;
        void Awake()
        {
            monsters = new HashSet<Monster>();
            BattleManager.inst.monsters = monsters;
            for (int i = 0; i < GameData.targetNode.monsterInfo.Count; i++)
            {
                var info = GameData.targetNode.monsterInfo[i];
                GameObject obj = Instantiate(MonsterGen.inst.list[info.Item1], areas[i]);
                if (obj.TryGetComponent<Monster>(out Monster monster))
                {
                    monster.level = info.Item2;
                    monster.isBattle = true;
                    monster.DeathAlarm += () => OnMonsterDeath(monster);
                    monster.ActionEndAlarm += OnMonsterActionEnd;
                    monsters.Add(monster);
                }
            }
        }

        public void OnMonsterDeath(Monster monster)
        {
            monsters.Remove(monster);
            if (monsters.Count == 0)
            {
                MonsterWipeoutEvent?.Invoke();
                GameData.ClearTargetNode();
            }
        }

        public void OnMonsterTurn()
        {
            monsterActionCount = monsters.Count;
            foreach (Monster monster in monsters)
            {
                monster.OnMoveAttack();
            }
        }

        public void OnMonsterActionEnd()
        {
            if(--monsterActionCount == 0) MonsterActionEndEvent?.Invoke();
        }
    }
}