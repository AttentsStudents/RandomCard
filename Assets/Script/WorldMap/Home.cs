using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Home : Island
    {
        void Start()
        {
            sprite = SpriteManager.instance.icon.home;
            ViewOnTheMap();
        }
    }
}

