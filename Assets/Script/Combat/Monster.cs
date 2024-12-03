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
        Debug.Log($"{name}��(��) {dmg}�� �������� �Ծ����ϴ�.");
        if (curHp <= 0)
        {
            OnDead();
        }
    }

    public void Initialize(int id, int lv, MonsterData data, GameObject target)
    {
        monsterId = id;
        level = lv;
        float maxHP = data.maxHP + (level * 5); // ������ ���� ü�� ����
        float armor = data.armor;
        float attack = data.attack + (level * 2); // ������ ���� ���ݷ� ����

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP;

        Target = target;
        transform.LookAt(Target.transform); // Ÿ�� �ٶ󺸱�
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
                statusEffects[statusName] = duration; // ���� ���� ����
            }
            else
            {
                statusEffects.Add(statusName, duration);
                StartCoroutine(HandleStatusEffect(statusName, duration));
            }
        }

    private IEnumerator HandleStatusEffect(string statusName, float duration)
    {
        Debug.Log($"{name}��(��) ���� �̻� '{statusName}'�� �ɷȽ��ϴ�.");
        while (duration > 0)
        {
            switch (statusName)
            {
                case "Poison":
                    OnDamage(5); // ���� 5 ������
                    break;

                case "Burn":
                    OnDamage(10); // ���� 10 ������
                    break;

                case "Freeze":
                    Debug.Log($"{name}��(��) ���پ� �ൿ�� �� �����ϴ�.");
                    yield break; // �ൿ ����
            }

            duration -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }

        statusEffects.Remove(statusName);
        Debug.Log($"{name}�� ���� �̻� '{statusName}'�� ����Ǿ����ϴ�.");
    }

}