using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager inst { get; set; }
        public enum Turn
        {
            PLAYER,
            MONSTER
        }
        public Turn turn { get; set; }
        public uint recoveryCost = 3;
        public UnityEvent CostObserb;
        public UnityAction MonsterAction { get; set; }

        void Start()
        {
            inst = this;
            turn = Turn.PLAYER;
            GameData.playerStat.cost = GameData.playerStat.maxCost;
        }

        public void OnTurnChange()
        {
            if (turn == Turn.PLAYER) MonsterTurn();
            else PlayerTurn();
            
        }

        public void OnPlayerTurnSkip()
        {
            if (turn == Turn.PLAYER) MonsterTurn();
        }

        void PlayerTurn()
        {
            turn = Turn.PLAYER;
            GameData.playerStat.cost += recoveryCost;
            if (GameData.playerStat.cost > GameData.playerStat.maxCost) GameData.playerStat.cost = GameData.playerStat.maxCost;
            CostObserb?.Invoke();
        }

        void MonsterTurn()
        {
            turn = Turn.MONSTER;
        }
    }
}

