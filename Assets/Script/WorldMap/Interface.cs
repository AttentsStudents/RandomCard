using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CheonJiWoon
{
    public interface IClickAction
    {
        UnityAction ClickAction { get; set; }
    }
    public interface ICloseAction
    {
        public UnityAction CloseAction { get; set; }
    }
}
