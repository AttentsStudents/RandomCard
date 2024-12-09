using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CheonJiWoon
{
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
    }



    public partial class ObjectManager : MonoBehaviour
    {
        public static ObjectManager instance;
        public Monster monster;
        public Island island;
        public GameObject tresure;
        public GameObject rest;

        void Awake() => instance = this;
    }
}
