using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CheonJiWoon
{
    public class MiddleIsland : Island, IClickAction, ICrashAction
    {
        public UnityAction ClickAction { get; set; }
        public UnityAction CrashAction { get; set; }
        public GameObject crashTarget { get; set; }

        public Transform center;
        public Transform left;
        public Transform right;
        Queue<GameObject> objectQueue = new Queue<GameObject>();


        void Start()
        {
            SetMapIcon();
            Generate();
            ViewOnTheMap();
        }
        void SetMapIcon()
        {
            switch (myNode.type)
            {
                case Node.Type.MONSTER:
                    sprite = SpriteManager.instance.icon.monster;
                    break;
                case Node.Type.TREASURE:
                    sprite = SpriteManager.instance.icon.treasure;
                    break;
                case Node.Type.REST:
                    sprite = SpriteManager.instance.icon.rest;
                    break;
            }
            if (myNode.clear) colorAlpha = 0.4f;
        }

        protected virtual void Generate()
        {
            if (myNode.clear) return;
            switch (myNode.type)
            {
                case Node.Type.MONSTER:
                    if (myNode.monsterInfo == null)
                    {
                        myNode.monsterInfo = new List<(int, int)>();

                        int count = Random.Range(1, 4);

                        while (count-- > 0)
                        {
                            RandomMonster();
                        }
                    }

                    int order = 0;
                    foreach ((int, int) monster in myNode.monsterInfo)
                    {
                        switch (order)
                        {
                            case 0:
                                CreateMonster(monster.Item1, center);
                                break;
                            case 1:
                                CreateMonster(monster.Item1, left);
                                break;
                            case 2:
                                CreateMonster(monster.Item1, right);
                                break;
                        }
                        order++;
                    }

                    break;
                case Node.Type.TREASURE:
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.instance.tresure, center));
                    break;
                case Node.Type.REST:
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.instance.rest, center));
                    break;
            }
            CrashAction = CrashActionMyType;
        }

        void RandomMonster()
        {
            int idx = Random.Range(0, MonsterGen.instance.list.Length);
            int level = Random.Range(1, 15);
            myNode.monsterInfo.Add((idx, level));
        }

        void CreateMonster(int idx, Transform tr)
        {
            GameObject monster = Instantiate<GameObject>(MonsterGen.instance.list[idx], tr);
            monster.GetComponentInChildren<Animator>().enabled = false;
            objectQueue.Enqueue(monster);
        }

        void CrashActionMyType()
        {
            myNode.clear = true;
            switch (myNode.type)
            {
                case Node.Type.MONSTER:
                    GameData.enemies = myNode.monsterInfo;
                    //SceneManager.LoadSceneAsync(0);
                    {
                        IHpObserve targetHp = crashTarget.GetComponent<IHpObserve>();
                        if (targetHp != null) targetHp.HpObserve?.Invoke(-5);
                    }
                    break;
                case Node.Type.TREASURE:
                    break;
                case Node.Type.REST:
                    {
                        IHpObserve targetHp = crashTarget.GetComponent<IHpObserve>();
                        if (targetHp != null) targetHp.HpObserve?.Invoke(5);
                    }
                    break;
            }
            colorAlpha = 0.4f;
            UpdateMapIcon.Invoke();
            GameData.SaveData();
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
