using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldMap {
    public class Map : MonoBehaviour
    {
        public static Map instance { get; private set; }
        public Camera mapCamera;
        public Transform content;
        void Awake()
        {
            instance = this;
        }
    }
}


