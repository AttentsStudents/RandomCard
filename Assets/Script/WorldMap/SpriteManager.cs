using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon {
    public partial class SpriteManager
    {
        [Serializable]
        public struct Icon
        {
            public Sprite monster;
            public Sprite treasure;
            public Sprite rest;
            public Sprite boss;
            public Sprite home;
        }
    }

    

    public partial class SpriteManager : MonoBehaviour
    {
        public static SpriteManager instance;
        public Icon icon;

        
        void Awake() => instance = this;
    }
}

