using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BattleSystem
{
    public int monsterId;
    public int level;

    public void Initialize(int id, int lv, MonsterData data,GameObject target)
    {
        monsterId = id;
        level = lv;
        float maxHP = data.maxHP + (level * 5); // ������ ���� ü�� ����
        float armor = data.armor;
        float attack = data.attack + (level * 2); // ������ ���� ���ݷ� ����

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP;

        Target = target;
    }

    public void DisplayStats()
    {
        Debug.Log($"Monster ID: {monsterId}, Level: {level}, HP: {battleStat.curHP}, Armor: {battleStat.Armor}, Attack: {battleStat.Attak}");
    }
}
