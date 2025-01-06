using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BattleStat
{
    public float maxHP;
    public float curHP;
    public float attack;
    public float armor;

    public BattleStat(float hp, float armor, float attack)
    {
        maxHP = hp;
        curHP = hp;
        this.armor = armor;
        this.attack = attack;
    }
}

[Serializable]
public class PlayerStat : BattleStat
{
    public uint maxCost;
    public uint cost;
    public uint recoveryCost;
    public uint gold;
    public PlayerStat(float hp, float armor, float attack, uint cost) : base(hp, armor, attack)
    {
        maxCost = cost;
        this.cost = cost;
        recoveryCost = 3;
        gold = 0;
    }
}

public abstract class BattleSystem : AnimProperty, IBattleObserve, IDeathAlarm
{
    public UnityAction HpObserve { get; set; }
    public BattleStat battleStat { get; set; }
    public UnityAction DeathAlarm { get; set; }
    bool isDead = false;
    Coroutine materialEffect;

    Renderer _render;
    Material orgMaterial;
    public Renderer render
    {
        get
        {
            if (_render == null)
            {
                _render = GetComponentInChildren<Renderer>();
                orgMaterial = _render.material;
            }
            return _render;
        }
        set
        {
            _render = value;
        }
    }

    public void OnDamage(float damage)
    {
        if (isDead) return;
        float delta = damage - battleStat.armor;
        DamageText damageText = Instantiate(ObjectManager.inst.damageText, CanvasCustom.main.lastLoad).GetComponent<DamageText>();
        if (delta < 0.0f)
        {
            delta = 0.0f;
        }
        damageText.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        if (delta > 0.0f)
        {
            HpChange(-delta);
            InstantiateEffect(ObjectManager.inst.effect.hit, transform.up * 0.5f);
            SoundBox.PlayOneShot(ObjectManager.inst.effect.hitSound);
            MaterialEffect(ObjectManager.inst.material.damage);

            if (Mathf.Approximately(battleStat.curHP, 0.0f)) Dead();
            else anim.SetTrigger(AnimParams.OnDamage);

            damageText.text.text = delta.ToString();
        }
        else
        {
            InstantiateEffect(ObjectManager.inst.effect.shield, transform.forward * 0.3f + transform.up * 0.5f);
            SoundBox.PlayOneShot(ObjectManager.inst.effect.shieldSound);
            MaterialEffect(ObjectManager.inst.material.shield);
            damageText.text.color = Color.white;
            damageText.text.text = "방어 성공!!";
        }

    }
    public void OnDamageNoMotion(float damage)
    {
        HpChange(damage);
        MaterialEffect(ObjectManager.inst.material.damage);

        if (Mathf.Approximately(battleStat.curHP, 0.0f)) Dead();
    }
    public void OnRecovery(float recovery)
    {

        InstantiateEffect(ObjectManager.inst.effect.heal, transform.up * 0.5f);
        SoundBox.PlayOneShot(ObjectManager.inst.effect.healSound);
        MaterialEffect(ObjectManager.inst.material.heal);
        HpChange(recovery);
    }



    public void OnBuff()
    {
        InstantiateEffect(ObjectManager.inst.effect.buff, transform.up * 0.5f);
        SoundBox.PlayOneShot(ObjectManager.inst.effect.buffSound);
        MaterialEffect(ObjectManager.inst.material.buff);
    }

    void HpChange(float value)
    {
        battleStat.curHP = Mathf.Clamp(battleStat.curHP + value, 0.0f, battleStat.maxHP);
        HpObserve?.Invoke();
    }

    void InstantiateEffect(GameObject effect, Vector3 pos)
    {
        GameObject obj = Instantiate(effect, transform);
        obj.transform.Translate(pos);
    }

    void MaterialEffect(Material material)
    {
        if (materialEffect != null) StopCoroutine(materialEffect);
        materialEffect = StartCoroutine(MaterialEffectCoroutine(material));
    }

    IEnumerator MaterialEffectCoroutine(Material material)
    {
        render.material = material;
        yield return WaitForSecondsCustom.Get(0.75f);
        render.material = orgMaterial;
    }
    void Dead()
    {
        DeathAlarm?.Invoke();
        anim.SetTrigger(AnimParams.OnDead);
        isDead = true;
    }
}

