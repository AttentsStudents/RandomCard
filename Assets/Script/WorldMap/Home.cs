using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Home : VisibleOnTheMap
    {
        void Start()
        {
            sprite = SpriteManager.instance.icon.home;
            ViewOnTheMap();
        }
    }
}

