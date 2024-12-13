using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class VisibleHpBar : MonoBehaviour, IBattleStat, IHpObserve
{
    public virtual BattleStat battleStat { get; set; }
    public UnityAction<float> HpObserve { get; set; }

    protected void AddHpBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/HpBar"), GameObject.FindWithTag("Canvas").transform);
        if (obj.TryGetComponent(out HpBar hpBar)) hpBar.target = gameObject;
    }
}
