using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Island : VisibleOnTheMap
    {
        public Node myNode { get; set; }
        void Start()
        {
            sprite = SpriteManager.instance.icon.home;
            ViewOnTheMap();
        }
    }
}

