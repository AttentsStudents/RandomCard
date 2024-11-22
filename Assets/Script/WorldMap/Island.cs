using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static CheonJiWoon.ObjectManager;

namespace CheonJiWoon
{
    public class Island : Home, IClickAction, ICrashAction
    {

        public enum Type
        {
            MONSTER,
            TREASURE,
            REST
        }
        Type myType;

        public UnityAction ClickAction { get; set; }
        public UnityAction CrashAction { get; set; }
        public GameObject crashTarget { get; set; }

        public Transform center;
        public Transform left;
        public Transform right;
        List<(int, int)> monsterInfo = new List<(int, int)>();
        Queue<GameObject> objectQueue = new Queue<GameObject>();


        void Start()
        {
            RandomMyType();
            Generate();
            ViewOnTheMap();
            CrashAction = CrashActionMyType;
        }
        protected virtual void Generate()
        {

            switch (myType)
            {
                case Type.MONSTER:
                    sprite = SpriteManager.instance.icon.monster;
                    int count = Random.Range(1, 4);

                    RandomMonsterGen(center);
                    if (count >= 2) RandomMonsterGen(left);
                    if (count >= 3) RandomMonsterGen(right);
                    break;
                case Type.TREASURE:
                    sprite = SpriteManager.instance.icon.treasure;
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.instance.tresure, center));
                    break;
                case Type.REST:
                    sprite = SpriteManager.instance.icon.rest;
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.instance.rest, center));
                    break;
            }
        }

        void RandomMonsterGen(Transform tr)
        {
            int idx = Random.Range(0, MonsterGen.instance.list.Length);
            int level = Random.Range(1, 15);

            GameObject monster = Instantiate<GameObject>(MonsterGen.instance.list[idx], tr);
            monster.GetComponentInChildren<Animator>().enabled = false;

            objectQueue.Enqueue(monster);
            monsterInfo.Add((idx, level));
        }

        void RandomMyType()
        {
            int random = Random.Range(1, 7);
            if (random < 5) myType = Type.MONSTER;
            else if (random < 6) myType = Type.TREASURE;
            else myType = Type.REST;
        }

        void CrashActionMyType()
        {
            switch (myType)
            {
                case Type.MONSTER:
                    GameData.enemies = monsterInfo;
                    SceneManager.LoadSceneAsync(0);
                    break;
                case Type.TREASURE:
                    break;
                case Type.REST:
                    IHpObserve targetHp = crashTarget.GetComponent<IHpObserve>();
                    if (targetHp != null) targetHp.HpObserve?.Invoke(5);
                    break;
            }
            DestroyObjects();
        }

        void DestroyObjects()
        {
            Instantiate(ObjectManager.instance.effect.Boom, center);
            while (objectQueue.Count > 0)
            {
                Destroy(objectQueue.Dequeue());
            }
        }
    }
}
