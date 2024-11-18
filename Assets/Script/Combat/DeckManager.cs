using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

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

public class CardEffects : MonoBehaviour
{
    public void AttackEffect(BattleSystem attacker, BattleSystem target)
    {
        float damage = attacker.battleStat.Attak;
        target.OnDamage(damage);
        Debug.Log($"[ȿ�� �ߵ�] {attacker.gameObject.name}��(��) {target.gameObject.name}���� {damage}�� ���ظ� �������ϴ�!");
    }

    public void DefenseEffect(BattleSystem defender, BattleSystem _)
    {
        float defenseValue = 5;
        defender.Armor += defenseValue;
        Debug.Log($"[ȿ�� �ߵ�] {defender.gameObject.name}��(��) �� {defenseValue} �������׽��ϴ�!");
    }

    public void HealEffect(BattleSystem healer, BattleSystem _)
    {
        float healValue = 10;
        healer.curHp += healValue;
        Debug.Log($"[ȿ�� �ߵ�] {healer.gameObject.name}��(��) ü���� {healValue} ȸ���߽��ϴ�!");
    }
}


public class DeckManager : MonoBehaviour
{
    public CardEffects cardEffects;
    public List<Card> deck = new List<Card>(); // ���� ��
    public List<Card> hand = new List<Card>(); // ���� ���� ����Ʈ
    public int drawCount = 5; // �� ���� �̴� ī�� ��
    private void Start()
    {
        LoadCardsFromJson("cards.json");
        ShowDeck();
    }

    private void LoadCardsFromJson(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            List<CardData> cardDataList = JsonUtility.FromJson<CardDataList>($"{{\"cards\":{jsonContent}}}").cards;

            foreach (var cardData in cardDataList)
            {
                CardType cardType = (CardType)System.Enum.Parse(typeof(CardType), cardData.cardType);
                Card newCard = new Card(cardData.cardName, cardType, cardData.energyCost, cardData.description, null);
                deck.Add(newCard);
            }

            Debug.Log("JSON ���Ͽ��� ī�� �����͸� �ε��߽��ϴ�.");
        }
        else
        {
            Debug.LogError($"������ ã�� �� �����ϴ�: {filePath}");
        }
    }

    private void ShowDeck()
    {
        foreach (var card in deck)
        {
            Debug.Log($"ī�� �̸�: {card.cardName}, ����: {card.cardType}, ������ ���: {card.energyCost}, ����: {card.description}");
        }
    }
    public void RerollCards() //���� �ϴ°� �����ؾߴ�
    {

    }
    public void DiscardCard(Card card)
    {
        if (deck.Contains(card))
        {
            deck.Remove(card); // ������ ����
            Debug.Log($"������ ī�� {card.cardName}�� �������ϴ�.");
        }
        else
        {
            Debug.Log("���� ���� ī���Դϴ�.");
        }
    }

    public void ShowHand()
    {
        Debug.Log("���� ����:");
        foreach (var card in hand)
        {
            Debug.Log($"ī�� �̸�: {card.cardName}, ����: {card.cardType}, ������ ���: {card.energyCost}, ����: {card.description}");
        }
    }

}
