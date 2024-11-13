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

        // 레벨에 따른 BattleStat 초기화
        float maxHP = 50 + (level * 5);
        float armor = 10; 
        float attack = 10 + (level * 5); // 레벨에 비례하여 공격력 설정

        battleStat = new BattleStat(maxHP, armor, attack);
        curHp = battleStat.maxHP; // 현재 체력 설정
    }

    public void DisplayStats()
    {
        Debug.Log($"Monster ID: {monsterId}, Level: {level}, HP: {battleStat.curHP}, Armor: {battleStat.Armor}, Attack: {battleStat.Attak}");
    }
}