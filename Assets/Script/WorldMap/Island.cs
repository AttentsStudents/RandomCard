using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CheonJiWoon
{
    public interface IClickAction
    {
        UnityAction ClickAction { get; set; }
    }
    public class Island : MonoBehaviour, IClickAction
    {
        public UnityAction ClickAction { get; set; }
    }
}
