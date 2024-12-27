using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace WorldMap
{
    public class MiddleIsland : Island, IClickAction, ICrashAction
    {
        public UnityAction ClickAction { get; set; }
        public UnityAction CrashAction { get; set; }
        public GameObject crashTarget { get; set; }

        public Transform center;
        public Transform left;
        public Transform right;
        public AudioSource audioSrc;
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
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.inst.tresure, center));
                    break;
                case Node.Type.REST:
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.inst.rest, center));
                    break;
            }
            CrashAction = CrashActionMyType;
        }

        void RandomMonster() => RandomMonster(1, 15);
        protected void RandomMonster(int min, int max)
        {
            int idx = Random.Range(0, MonsterGen.inst.list.Length);
            int level = Random.Range(min, max + 1);
            myNode.monsterInfo.Add((idx, level));
        }

        protected void CreateMonster(int idx, Transform tr)
        {
            GameObject monster = Instantiate<GameObject>(MonsterGen.inst.list[idx], tr);
            monster.GetComponentInChildren<Animator>().enabled = false;
            objectQueue.Enqueue(monster);
        }

        void CrashActionMyType()
        {
            GameData.targetNode = myNode;
            if(audioSrc != null)
            {
                audioSrc.time = 0.2f;
                audioSrc.Play();
            }
            switch (myNode.type)
            {
                case Node.Type.MONSTER:
                    Loading.LoadScene(Scene.BATTLE);
                    //{
                    //    IHpObserve targetHp = crashTarget.GetComponent<IHpObserve>();
                    //    if (targetHp != null) targetHp.HpObserve?.Invoke(-5);
                    //    GameData.ClearTargetNode();
                    //}
                    break;
                case Node.Type.TREASURE:
                    Instantiate(Resources.Load($"{SceneData.prefabPath}/Tresure"),
                        WorldMapCanvas.inst.transform);
                    break;
                case Node.Type.REST:
                    {
                        if(crashTarget.TryGetComponent<IBattleObserve>(out IBattleObserve battleObserve))
                        {
                            battleObserve.battleStat.curHP = Mathf.Clamp(battleObserve.battleStat.curHP + 5, 0, battleObserve.battleStat.maxHP);
                            battleObserve.HpObserve.Invoke();
                        }
                        GameData.ClearTargetNode();
                    }
                    break;
            }
            colorAlpha = 0.4f;
            UpdateMapIcon.Invoke();
            DestroyObjects();
        }

        void DestroyObjects()
        {
            Instantiate(ObjectManager.inst.effect.boom, center);
            while (objectQueue.Count > 0) { Destroy(objectQueue.Dequeue()); }
        }
    }
}
