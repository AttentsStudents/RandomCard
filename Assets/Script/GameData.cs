using CheonJiWoon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
class SaveDataField
{
    int[] cards;
    float maxHP;
    float curHP;
    float Cost;
    float Armor;
    float Attak;

    public SaveDataField()
    {
        cards = GameData.cards;
        maxHP = GameData.playerStat.maxHP;
        curHP = GameData.playerStat.curHP;
        Cost = GameData.playerStat.Cost;
        Armor = GameData.playerStat.Armor;
        Attak = GameData.playerStat.Attack;
    }

    public void Load()
    {
        GameData.cards = cards;
        GameData.playerStat.maxHP = maxHP;
        GameData.playerStat.curHP = curHP;
        GameData.playerStat.Cost = Cost;
        GameData.playerStat.Armor = Armor;
        GameData.playerStat.Attak = Attak;
    }
}

public static class GameData
{
    public static WorldMapInfo world { get; set; }
    public static int[] cards = new int[50];
    public static List<(int, int)> enemies { get; set; }
    public static BattleStat playerStat;
    public static Node playerNode { get; set; }

    static BinaryFormatter bf = new BinaryFormatter();

    public static void SaveData()
    {
        using (FileStream fs = new FileStream($"{Application.dataPath}/save.gsvdata", FileMode.Create))
        {
            bf.Serialize(fs, new SaveDataField());
        }
    }

    public static void LoadData()
    {
        using (FileStream fs = new FileStream($"{Application.dataPath}/save.gsvdata", FileMode.Open))
        {
            (bf.Deserialize(fs) as SaveDataField)?.Load();
        }
    }
}