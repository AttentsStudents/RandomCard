using WorldMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class VisibleHpBar : BattleSystem
{
    protected void AddHpBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>($"Prefabs/HpBar"), GameObject.FindWithTag("Canvas").transform);
        if (obj.TryGetComponent(out HpBar hpBar))
        {
            hpBar.target = gameObject;
            DeathAlarm += () => Destroy(obj);
        }
    }
}
