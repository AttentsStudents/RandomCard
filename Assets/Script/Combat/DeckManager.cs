using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CardData
{
    public string cardName;
    public string cardType; // JSON �����ʹ� ���ڿ��� �����
    public int energyCost;
    public string description;
}

[System.Serializable]
public class CardDataList
{
    public List<CardData> cards;
}


public enum CardType { Attack, Defense, Skill }

[System.Serializable]
public class Card
{
    public string cardName;
    public CardType cardType;
    public int energyCost;
    public string description;
    public string imagename;
    public System.Action<BattleSystem, BattleSystem> effect;

    public Card(string name, CardType type, int cost, string desc, System.Action<BattleSystem, BattleSystem> cardEffect)
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
    public CardEffects cardEffects;
    public List<Card> deck = new List<Card>(); // ���� ��
    public List<Card> hand = new List<Card>(); // ���� ���� ����Ʈ
    public int drawCount = 5; // �� ���� �̴� ī�� �� ���� ���� ���

    private void Start()
    {
        LoadCardsFromJson("cards.json");
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

    private void DrawCards(int count)
    {
        for (int i = 0; i < count && deck.Count > 0; i++)
        {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        Debug.Log("ī�带 �̾ҽ��ϴ�.");
    }

    private void LoadCardsFromJson(string fileName)
    {
        // ���� ��� ����
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ�: {filePath}");
            return;
        }

        try
        {
            // JSON ���� �б�
            string jsonContent = File.ReadAllText(filePath);

            // JSON �Ľ�
            CardDataList cardDataList = JsonUtility.FromJson<CardDataList>(jsonContent);

            if (cardDataList == null || cardDataList.cards == null || cardDataList.cards.Count == 0)
            {
                Debug.LogWarning("JSON �����Ͱ� ��� �ְų� ��ȿ���� �ʽ��ϴ�.");
                return;
            }

            foreach (var cardData in cardDataList.cards)
            {
                Debug.Log($"ī�� ������ �ε�: �̸�={cardData.cardName}, Ÿ��={cardData.cardType}, ���={cardData.energyCost}");

                // ī�� Ÿ�� �Ľ�
                if (!System.Enum.TryParse(cardData.cardType, out CardType cardType))
                {
                    Debug.LogError($"�� �� ���� ī�� Ÿ��: {cardData.cardType}");
                    continue;
                }

                // ī�� ȿ�� ����
                System.Action<BattleSystem, BattleSystem> effect = GetEffectByType(cardType, cardEffects);

                if (effect == null)
                {
                    Debug.LogError($"ȿ���� ���� ī��: {cardData.cardName}");
                    continue;
                }

                // ī�� ���� �� �߰�
                Card newCard = new Card(cardData.cardName, cardType, cardData.energyCost, cardData.description, effect)
                {
                    imagename = cardData.cardName // ī�� �̸��� �̹��� �̸����� ���� (�ʿ信 ���� ���� ����)
                };

                deck.Add(newCard);
                Debug.Log($"���� �߰��� ī��: {newCard.cardName}");
            }

            Debug.Log($"�� {deck.Count}���� ī�尡 ���� �߰��Ǿ����ϴ�.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON �Ľ� �� ���� �߻�: {e.Message}");
        }

        // Inspector ���� (�ʿ�� ȣ��)
        UpdateInspector();
    }





    public System.Action<BattleSystem, BattleSystem> GetEffectByType(CardType type, CardEffects cardEffects)
    {
        switch (type)
        {
            case CardType.Attack:
                return cardEffects.AttackEffect;
            case CardType.Defense:
                return cardEffects.DefenseEffect;
            case CardType.Skill:
                return cardEffects.HealEffect;
            default:
                Debug.LogError("�� �� ���� ī�� Ÿ���Դϴ�!");
                return null;
        }
    }
    private void UpdateInspector()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
