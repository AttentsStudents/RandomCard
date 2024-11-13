using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CardType { Attack, Defense, Skill }

[System.Serializable]
public class Card
{
    public string cardName;
    public CardType cardType;
    public int energyCost;
    public string description;
    public UnityAction<Card> effect;

    public Card(string name, CardType type, int cost, string desc, UnityAction<Card> cardEffect)
    {
        cardName = name;
        cardType = type;
        energyCost = cost;
        description = desc;
        effect = cardEffect;
    }
}

public class DeckManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public int drawCount = 5; // 턴마다 뽑을 카드 수
    public int rerollPoints = 3; // 리롤 포인트 초기 설정

    // 핸드에 카드 뽑기
    public void DrawCards()
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (deck.Count > 0)
            {
                Card drawnCard = deck[0];
                hand.Add(drawnCard);
                deck.RemoveAt(0);
            }
        }
    }

    // 리롤 기능
    public void RerollHand()
    {
        if (rerollPoints > 0)
        {
            // 리롤 포인트 소모
            rerollPoints--;

            // 기존 핸드의 카드를 덱으로 되돌림
            foreach (Card card in hand)
            {
                deck.Add(card);
            }

            hand.Clear();

            // 새롭게 DrawCards를 호출하여 새로운 핸드 구성
            DrawCards();
        }
        else
        {
            Debug.Log("리롤 포인트가 부족합니다!");
        }
    }
}



