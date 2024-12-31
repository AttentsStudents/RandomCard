using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class ObjectManager
{
    [Serializable]
    public struct Monster
    {
        public GameObject beholder;
        public GameObject chest;
        public GameObject fish;
        public GameObject golem;
        public GameObject lich;
        public GameObject moskito;
        public GameObject orc;
    }

    [Serializable]
    public struct Island
    {
        public GameObject home;
        public GameObject normal;
        public GameObject boss;
    }

    [Serializable]
    public struct Effect
    {
        public GameObject boom;
        public GameObject hit;
        public GameObject heal;
        public GameObject buff;
        public GameObject shield;
    }

    [Serializable]
    public struct Materials
    {
        public Material shield;
        public Material buff;
        public Material heal;
        public Material damage;
    }
}



public partial class ObjectManager : SingleTon<ObjectManager>
{
    public Monster monster;
    public Island island;
    public GameObject tresure;
    public GameObject rest;
    public Effect effect;
    public Materials material;
    public GameObject damageText;
    public GameObject alertMessage;
    public GameObject hpBar;
    public GameObject enemyCollider;
    void Awake() => Init();
}
