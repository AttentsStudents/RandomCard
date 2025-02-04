using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cost : MonoBehaviour
{
    public TMP_Text text;
    public Image fill;
    public UnityEvent CostZeroEvent;

    void Start()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        text.text = GameData.playerStat.cost.ToString();
        fill.fillAmount = ((float)GameData.playerStat.cost / (float)GameData.playerStat.maxCost);
        if (GameData.playerStat.cost == 0) CostZeroEvent?.Invoke();
    }
}
