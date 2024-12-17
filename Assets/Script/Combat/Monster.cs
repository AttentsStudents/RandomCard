using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Monster : VisibleHpBar
    {
        public MonsterData data;
        public int level { get; set; }
        public bool isBattle { get; set; }

        void Awake()
        {
            battleStat = new BattleStat(data.maxHP, data.armor, data.attack);
        }
        void Start()
        {
            if (isBattle)
            {
                AddHpBar();
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyCollider"), GameObject.FindWithTag("Canvas").transform);
                if(obj.TryGetComponent<EnemyCollider>(out EnemyCollider enemyCollider))
                {
                    enemyCollider.target = gameObject;
                    DeathAlarm += () => Destroy(obj);
                }
            }
        }

        void Update()
        {

        }
    }
}

