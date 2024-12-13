using System.Collections.Generic;
using UnityEngine;
using static CardEffects;

public class DeckManager : MonoBehaviour
{
    public List<ItemCard> cardDataScriptableObjects; // ScriptableObject ����Ʈ
    public List<Card> deck = new List<Card>(); // ���� ��
    public List<Card> hand = new List<Card>(); // ���� ���� ����Ʈ
    public int drawCount = 3; // ��ο� ī�� ��
    public static int Atp, Dep, Sp = 0; // ī�� ī��Ʈ

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
            if (deck[0].cardType == CardType.Attack) Atp++;
            if (deck[0].cardType == CardType.Defense) Dep++;
            if (deck[0].cardType == CardType.Skill) Sp++;

            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
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

            // ī�� Ÿ�� ����
            CardType cardType = GetCardTypeFromByte(itemCard.type);

            // ī�� ����
            Card newCard = new Card(
                itemCard.card,
                cardType,
                itemCard.description,
                null // ���� ȿ���� ���ǵ��� ����
            )
            {
                effectValue = itemCard.figure, // ScriptableObject�� `figure` ������ �ʱ�ȭ
                sprite = itemCard.sprite       // ScriptableObject�� ��������Ʈ ����
            };

            // ���� �߰�
            deck.Add(newCard);
        }
    }


    private CardType GetCardTypeFromByte(byte type)
    {
        return type switch
        {
            0 => CardType.Attack,
            1 => CardType.Defense,
            2 => CardType.Skill,
            _ => throw new System.Exception($"�� �� ���� ī�� Ÿ��: {type}")
        };
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
    public string cardName;       // ī�� �̸�
    public CardType cardType;     // ī�� Ÿ��
    public string description;    // ī�� ����
    public ICardEffect effect;    // ī�� ȿ�� Ŭ����
    public Sprite sprite;         // ī�� �̹���
    public float effectValue;     // ī�� ȿ�� �� (��: ���ݷ�, ���� ��)

    public Card(string name, CardType type, string desc, ICardEffect cardEffect)
    {
        cardName = name;
        cardType = type;
        description = desc;
        effect = cardEffect;
    }
}
