using WorldMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class VisibleHpBar : BattleSystem
{
    protected void AddHpBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>($"Prefabs/HpBar"), CanvasCustom.main.priortyLoad);
        if (obj.TryGetComponent(out HpBar hpBar))
        {
            hpBar.target = gameObject;
            DeathAlarm += () => Destroy(obj);
        }
    }
}
