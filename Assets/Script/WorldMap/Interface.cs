using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CheonJiWoon
{
    public interface IClickAction
    {
        UnityAction ClickAction { get; set; }
    }
    public interface ICloseAction
    {
        UnityAction CloseAction { get; set; }
    }
    public interface ICrashAction
    {
        UnityAction CrashAction { get; set; }
        GameObject crashTarget { get; set; }
    }
    public interface IHpObserve
    {
        UnityAction<float> HpObserve { get; set; }
    }
}
