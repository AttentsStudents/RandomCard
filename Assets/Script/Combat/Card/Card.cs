using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public abstract class Card : MonoBehaviour
    {
        public abstract void Active(List<BattleSystem> battles);
    }
}

