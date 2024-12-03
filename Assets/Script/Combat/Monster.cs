using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BattleSystem, IDamage
{
    public int monsterId;
    public int level;

    public void OnDamage(float dmg)
    {
        curHp -= dmg;
        Debug.Log($"{name}이(가) {dmg}의 데미지를 입었습니다.");
        if (curHp <= 0)
        {
            OnDead();
        }
    }

    public void Initialize(int id, int lv, MonsterData data, GameObject target)
    {
        monsterId = id;
        level = lv;
        float maxHP = data.maxHP + (level * 5); // 레벨에 따른 체력 증가
        float armor = data.armor;
        float attack = data.attack + (level * 2); // 레벨에 따른 공격력 증가

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP;

        Target = target;
        transform.LookAt(Target.transform); // 타겟 바라보기
    }


    public void DisplayStats()
    {
        Debug.Log($"Monster ID: {monsterId}, Level: {level}, HP: {battleStat.curHP}, Armor: {battleStat.Armor}, Attack: {battleStat.Attak}");
    }
        private Dictionary<string, float> statusEffects = new Dictionary<string, float>();

        public void ApplyStatusEffect(string statusName, float duration)
        {
            if (statusEffects.ContainsKey(statusName))
            {
                statusEffects[statusName] = duration; // 기존 상태 갱신
            }
            else
            {
                statusEffects.Add(statusName, duration);
                StartCoroutine(HandleStatusEffect(statusName, duration));
            }
        }

    private IEnumerator HandleStatusEffect(string statusName, float duration)
    {
        Debug.Log($"{name}이(가) 상태 이상 '{statusName}'에 걸렸습니다.");
        while (duration > 0)
        {
            switch (statusName)
            {
                case "Poison":
                    OnDamage(5); // 매초 5 데미지
                    break;

                case "Burn":
                    OnDamage(10); // 매초 10 데미지
                    break;

                case "Freeze":
                    Debug.Log($"{name}이(가) 얼어붙어 행동할 수 없습니다.");
                    yield break; // 행동 제한
            }

            duration -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }

        statusEffects.Remove(statusName);
        Debug.Log($"{name}의 상태 이상 '{statusName}'이 종료되었습니다.");
    }

}