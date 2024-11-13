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
    public int drawCount = 5; // �ϸ��� ���� ī�� ��
    public int rerollPoints = 3; // ���� ����Ʈ �ʱ� ����

    // �ڵ忡 ī�� �̱�
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

    // ���� ���
    public void RerollHand()
    {
        if (rerollPoints > 0)
        {
            // ���� ����Ʈ �Ҹ�
            rerollPoints--;

            // ���� �ڵ��� ī�带 ������ �ǵ���
            foreach (Card card in hand)
            {
                deck.Add(card);
            }

            hand.Clear();

            // ���Ӱ� DrawCards�� ȣ���Ͽ� ���ο� �ڵ� ����
            DrawCards();
        }
        else
        {
            Debug.Log("���� ����Ʈ�� �����մϴ�!");
        }
    }
}



