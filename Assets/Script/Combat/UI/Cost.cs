using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cost : MonoBehaviour
{
    public TMP_Text text;
    public Image fill;

    void Start()
    {
        DataUpdate();
    }

    void DataUpdate()
    {
        text.text = GameData.playerStat.cost.ToString();
        fill.fillAmount = GameData.playerStat.cost / GameData.playerStat.maxCost;
    }
}
