using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static WorldMapInfo wolrdMapInfo {  get; set; }
    public static int[] cards = new int[50];
    public static List<(int, int)> enemies { get; set; }
    public static BattleStat playerStat;
    public static Node playerNode { get; set; }
}