using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class HpBarColors
{
    public static Color bgIdle = Color(50, 50, 50);
    public static Color bgPlus = Color(230, 230, 255);
    public static Color bgMinus = Color(0, 0, 0);
    public static Color fillIdle = Color(159, 0, 2);
    public static Color fillPlus = Color(200, 0, 20);
    public static Color fillMinus = Color(116, 2, 4);

    static Color Color(int r, int g, int b) => Color(r, g, b, 255);
    static Color Color(int r, int g, int b, int a) => new Color(F(r), F(g), F(b), F(a));
    static float F(int n) => (float)n / 255.0f;
}

public class HpBar : MonoBehaviour
{
    public GameObject target;
    public Image bgImg;
    public Image fillImg;
    BattleStat targetStat { get => target.GetComponent<IBattleStat>()?.battleStat; }
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
        slider.value = targetStat.curHP / targetStat.maxHP;
        text.text = $"{targetStat.curHP} / {targetStat.maxHP}";

        IHpObserve targetHpObserve = target.GetComponent<IHpObserve>();
        if (targetHpObserve != null) targetHpObserve.HpObserve += OnChange;

        bgImg.color = HpBarColors.bgIdle;
        fillImg.color = HpBarColors.fillIdle;
    }

    public void OnChange(float v)
    {
        if (change != null) StopCoroutine(change);
        change = StartCoroutine(Change(v));
    }

    IEnumerator Change(float v)
    {
        targetStat.curHP = Mathf.Clamp(targetStat.curHP + v, 0.0f, targetStat.maxHP);
        text.text = $"{targetStat.curHP} / {targetStat.maxHP}";
        float dist = targetStat.curHP / targetStat.maxHP - slider.value;
        float dir = 1.0f;
        if (dist < 0.0f)
        {
            dir = -1.0f;
            dist *= -1.0f;
            bgImg.color = HpBarColors.bgMinus;
            fillImg.color = HpBarColors.fillMinus;
        }
        else
        {
            bgImg.color = HpBarColors.bgPlus;
            fillImg.color = HpBarColors.fillPlus;
        }

        while (dist > 0.0f)
        {
            float delta = 0.1f * Time.deltaTime;
            if (delta > dist) delta = dist;
            slider.value += delta * dir;
            dist -= delta;
            yield return null;
        }

        bgImg.color = HpBarColors.bgIdle;
        fillImg.color = HpBarColors.fillIdle;
    }
}
