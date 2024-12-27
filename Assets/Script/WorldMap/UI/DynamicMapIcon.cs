using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap
{
    public class DynamicMapIcon : MapIcon
    {
        void Start() => Init();
        void Update() => UpdateTransform();
    }
}

