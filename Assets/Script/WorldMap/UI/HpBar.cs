using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public GameObject target;
    Slider _slider;
    Slider slider
    {
        get
        {
            if (_slider == null)
            {
                _slider = GetComponent<Slider>();
            }
            return _slider;
        }
    }
    public TMP_Text text;
    Coroutine change;

    void Start()
    {
        Init();
    }

    void Init()
    {
        GameData.playerStat.maxHP = GameData.playerStat.curHP = 60;
        GameData.playerStat.curHP = 30;
        slider.value = GameData.playerStat.curHP / GameData.playerStat.maxHP;
        text.text = $"{GameData.playerStat.curHP} / {GameData.playerStat.maxHP}";

        IHpObserve targetHp = target.GetComponent<IHpObserve>();
        if (targetHp != null) targetHp.HpObserve += OnChange;
    }

    public void OnChange(float v)
    {
        if (change != null) StopCoroutine(change);
        change = StartCoroutine(Change(v));
    }

    IEnumerator Change(float v)
    {
        GameData.playerStat.curHP = Mathf.Clamp(GameData.playerStat.curHP + v, 0.0f, GameData.playerStat.maxHP);
        text.text = $"{GameData.playerStat.curHP} / {GameData.playerStat.maxHP}";
        float dist = GameData.playerStat.curHP / GameData.playerStat.maxHP - slider.value;
        float dir = 1.0f;
        if (dist < 0.0f)
        {
            dir = -1.0f;
            dist *= -1.0f;
        }

        while (dist > 0.0f)
        {
            float delta = 0.5f * Time.deltaTime;
            if (delta > dist) delta = dist;
            slider.value += delta * dir;
            dist -= delta;
            yield return null;
        }
    }
}
