using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static Node[,] mapInfo;
    public static int[] cards = new int[50];
    public static List<(int, int)> enemies;
    public static BattleStat playerStat = new BattleStat();
}