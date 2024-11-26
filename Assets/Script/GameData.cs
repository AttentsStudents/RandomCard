using CheonJiWoon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
class SaveDataField
{
    List<int> cards;
    BattleStat playerStat;
    (int, int) playerPlace;
    List<(int, int)> mapKey;
    List<Node> mapNode;
    (int, int) firstNode;
    (int, int) lastNode;

    public SaveDataField()
    {
        cards = GameData.cards;
        playerStat = GameData.playerStat;
        playerPlace = GameData.playerNode.GetKey();

        mapKey = new List<(int, int)>();
        mapNode = new List<Node>();

        foreach(var item in Node.map)
        {
            mapKey.Add(item.Key);
            mapNode.Add(item.Value);
        }

        firstNode = Node.firstNode.GetKey();
        lastNode = Node.lastNode.GetKey();
    }

    public void Load()
    {
        GameData.cards = cards;
        GameData.playerStat = playerStat;

        Node.map = new Dictionary<(int, int), Node>();
        for (int i = 0; i < Mathf.Min(mapKey.Count, mapNode.Count); i++)
        {
            Node.map.Add(mapKey[i], mapNode[i]);
        }

        GameData.playerNode = Node.GetNode(playerPlace.Item1, playerPlace.Item2);
        Node.firstNode = Node.GetNode(firstNode.Item1, firstNode.Item2);
        Node.lastNode = Node.GetNode(lastNode.Item1, lastNode.Item2);
    }
}

public static class GameData
{
    public static List<int> cards { get; set; }
    public static List<(int, int)> enemies { get; set; }
    public static BattleStat playerStat { get; set; }
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

    public static void InitData()
    {
        cards = new List<int>();
        enemies = null;
        playerStat = null;
        playerNode = null;
    }
}