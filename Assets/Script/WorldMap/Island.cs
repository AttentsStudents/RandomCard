using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CheonJiWoon
{
    public class Island : MonoBehaviour, IClickAction
    {
        public UnityAction ClickAction { get; set; }
    }
}
