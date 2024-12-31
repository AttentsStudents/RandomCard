using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisibleHpBar : BattleSystem
{
    protected void AddHpBar()
    {
        GameObject obj = Instantiate(ObjectManager.inst.hpBar, CanvasCustom.main.priortyLoad);
        if (obj.TryGetComponent(out HpBar hpBar))
        {
            hpBar.target = gameObject;
            DeathAlarm += () => obj.SetActive(false);
        }
    }
}
