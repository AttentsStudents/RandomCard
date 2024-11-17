using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CheonJiWoon.ObjectManager;

namespace CheonJiWoon
{
    public class Island : VisibleOnTheMap, IClickAction, ICrashAction
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

        public Transform center;
        public Transform left;
        public Transform right;
        Queue<GameObject> objectQueue = new Queue<GameObject>();


        void Start()
        {
            Generate();
            ViewOnTheMap();
            CrashAction = DestroyObjects;
        }
        void Generate()
        {
            myType = (Type)Random.Range(0, 3);
            switch (myType)
            {
                case Type.MONSTER:
                    sprite = SpriteManager.instance.icon.monster;
                    int count = Random.Range(1, 4);

                    objectQueue.Enqueue(Instantiate<GameObject>(MonsterGen.instance.RandomMonster(), center));
                    if (count >= 2) objectQueue.Enqueue(Instantiate<GameObject>(MonsterGen.instance.RandomMonster(), left));
                    if (count >= 3) objectQueue.Enqueue(Instantiate<GameObject>(MonsterGen.instance.RandomMonster(), right));
                    break;
                case Type.TREASURE:
                    sprite = SpriteManager.instance.icon.treasure;
                    objectQueue.Enqueue(Instantiate<GameObject>(ObjectManager.instance.tresure, center));
                    break;
                case Type.REST:
                    sprite = SpriteManager.instance.icon.rest;
                    break;
            }
        }

        GameObject RandomMonster()
        {
            return MonsterGen.instance.list[Random.Range(0, MonsterGen.instance.list.Length)];
        }

        public void DestroyObjects()
        {
            while (objectQueue.Count > 0)
            {
                Destroy(objectQueue.Dequeue());
            }
        }
    }
}
