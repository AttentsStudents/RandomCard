using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager inst { get; set; }
        public enum Turn
        {
            PLAYER,
            MONSTER
        }
        public Turn turn { get; set; }
        public UnityEvent PlayerTurnEvent;
        public UnityEvent MonsterTurnEvent;
        public Player player { get; set; }
        public HashSet<Monster> monsters { get; set; }

        public Armor armor;

        public Cost cost;
        public Transform cards;

        void Awake()
        {
            inst = this;
        }

        void Start()
        {
            turn = Turn.PLAYER;
            GameData.playerStat.cost = GameData.playerStat.maxCost;
            AlertMessage.Alert("카드를 클릭/드래그해서 몬스터를 처치하세요");
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

        public void OnCheckCardsInHand()
        {
            if(cards.childCount == 0)
            {
                AlertMessage.Alert("턴을 자동 종료합니다 (손에 카드 부족)");
                OnPlayerTurnSkip();
            }
        }

        IEnumerator MonsterTurn()
        {
            yield return new WaitForSeconds(0.5f);
            MonsterTurnEvent?.Invoke();
        }
    }
}

