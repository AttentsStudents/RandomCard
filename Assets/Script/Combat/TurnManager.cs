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
        public UnityEvent PlayerTurnEvent;
        public UnityEvent MonsterTurnEvent;

        void Awake()
        {
            inst = this;
        }

        void Start()
        {
            turn = Turn.PLAYER;
            GameData.playerStat.cost = GameData.playerStat.maxCost;
        }


        public void OnPlayerTurnSkip()
        {
            if (turn == Turn.PLAYER) OnMonsterTurn();
        }

        public void OnPlayerTurn()
        {
            turn = Turn.PLAYER;
            GameData.playerStat.cost += GameData.playerStat.recoveryCost;
            if (GameData.playerStat.cost > GameData.playerStat.maxCost) GameData.playerStat.cost = GameData.playerStat.maxCost;
            PlayerTurnEvent?.Invoke();
        }

        public void OnMonsterTurn()
        {
            turn = Turn.MONSTER;
            StartCoroutine(MonsterTurn());
        }

        IEnumerator MonsterTurn()
        {
            yield return new WaitForSeconds(0.5f);
            MonsterTurnEvent?.Invoke();
        }
    }
}

