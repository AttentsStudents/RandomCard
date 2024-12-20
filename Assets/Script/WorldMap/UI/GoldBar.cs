using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldBar : MonoBehaviour
{
    public TMP_Text goldText;
    void Start()
    {
        goldText.text = GameData.playerStat.gold.ToString();
    }
}
