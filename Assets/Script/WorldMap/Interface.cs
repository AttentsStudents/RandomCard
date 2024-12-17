using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IClickAction
{
    UnityAction ClickAction { get; set; }
}
public interface ICrashAction
{
    UnityAction CrashAction { get; set; }
    GameObject crashTarget { get; set; }
}
public interface IHpObserve
{
    UnityAction HpObserve { get; set; }
}
public interface IBattleStat
{
    BattleStat battleStat { get; }
}
public interface IDeathAlarm
{
    UnityAction DeathAlarm { get; set; }
}

public interface IBattleObserve : IBattleStat, IHpObserve { }
