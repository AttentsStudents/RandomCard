using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BattleSystem
{
    public int monsterId;
    public int level;

    public void Initialize(int id, int lvl)
    {
        monsterId = id;
        level = lvl;

        // ������ ���� BattleStat �ʱ�ȭ
        float maxHP = 50 + (level * 5);
        float armor = 10; 
        float attack = 10 + (level * 5); // ������ ����Ͽ� ���ݷ� ����

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP; // ���� ü�� ����
    }

    public void DisplayStats()
    {
        Debug.Log($"Monster ID: {monsterId}, Level: {level}, HP: {battleStat.curHP}, Armor: {battleStat.Armor}, Attack: {battleStat.Attak}");
    }
}