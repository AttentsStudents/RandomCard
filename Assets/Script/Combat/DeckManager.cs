using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<ItemCard> cardDataScriptableObjects; // ���� ScriptableObject�� ������ ����Ʈ
    public List<Card> deck = new List<Card>(); // ���� ��
    public List<Card> hand = new List<Card>(); // ���� ���� ����Ʈ
    public int drawCount = 3; // �� ���� �̴� ī�� ��
    static public int Atp, Dep, Sp = 0;

    private void Start()
    {
        LoadCardsFromScriptableObjects(); // ScriptableObject���� ������ �ε�
        if (deck == null || deck.Count == 0)
        {
            Debug.LogError("���� ��� �ֽ��ϴ�. ī�� �����͸� Ȯ���ϼ���.");
        }
        else
        {
            Debug.Log($"���� �� {deck.Count}���� ī�尡 �ֽ��ϴ�.");
        }
        ShuffleDeck();
        DrawCards(drawCount);
    }

    public void RerollCards()
    {
        foreach (var card in hand)
        {
            deck.Add(card);
        }
        hand.Clear();
        ShuffleDeck();
        Atp = 0;
        Dep = 0;
        Sp = 0;
        DrawCards(drawCount);
        Debug.Log("ī�带 �����߽��ϴ�.");
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("���� �������ϴ�.");
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count && deck.Count > 0; i++)
        {
            // ī�� Ÿ�Կ� ���� �ڵ忡 ���� ī�� ī����
            if (deck[0].cardType == CardType.Attack)
            {
                Atp++;
            }
            if (deck[0].cardType == CardType.Defense)
            {
                Dep++;
            }
            if (deck[0].cardType == CardType.Skill)
            {
                Sp++;
            }
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        Debug.Log("ī�带 �̾ҽ��ϴ�.");
    }


    private void LoadCardsFromScriptableObjects()
    {
        if (cardDataScriptableObjects == null || cardDataScriptableObjects.Count == 0)
        {
            Debug.LogError("ScriptableObject �����Ͱ� ��� �ְų� ��ȿ���� �ʽ��ϴ�.");
            return;
        }

        foreach (var itemCard in cardDataScriptableObjects)
        {
            Debug.Log($"ī�� ������ �ε�: �̸�={itemCard.card}, Ÿ��={itemCard.type}, ����={itemCard.description}");

            // ī�� Ÿ�� ����
            CardType cardType = GetCardTypeFromByte(itemCard.type);

            // ī�� ���� �� �߰�
            Card newCard = new Card(
                itemCard.card,
                cardType,
                itemCard.description,
                null // ���� ȿ���� ���ǵ��� ����
            )
            {
                sprite = itemCard.sprite // ScriptableObject���� ��������Ʈ ����
            };
            //���� ī�� ���� �������� ī����
            deck.Add(newCard);
        }

        Debug.Log($"�� {deck.Count}���� ī�尡 ���� �߰��Ǿ����ϴ�.");
    }

    private CardType GetCardTypeFromByte(byte type)
    {
        // byte ���� CardType enum���� ����
        switch (type)
        {
            case 0:
                return CardType.Attack;
            case 1:
                return CardType.Defense;
            case 2:
                return CardType.Skill;
            default:
                Debug.LogError($"�� �� ���� ī�� Ÿ��: {type}");
                return CardType.Attack; // �⺻�� ����
        }
    }
}

public enum CardType
{
    Attack,
    Defense,
    Skill
}

[System.Serializable]
public class Card
{
    public string cardName; // ī�� �̸�
    public CardType cardType; // ī�� Ÿ��
    public string description; // ī�� ����
    public Sprite sprite; // ī�� �̹���
    public System.Action<BattleSystem, BattleSystem> effect; // ī�� ȿ�� (���� ���� ����)

    public Card(string name, CardType type, string desc, System.Action<BattleSystem, BattleSystem> cardEffect)
    {
        cardName = name;
        cardType = type;
        description = desc;
        effect = cardEffect;
    }
}
