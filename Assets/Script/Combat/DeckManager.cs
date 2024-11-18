using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

[System.Serializable]
public class CardData
{
    public string cardName;
    public string cardType; // JSON 데이터는 문자열로 저장됨
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
        Debug.Log($"[효과 발동] {attacker.gameObject.name}이(가) {target.gameObject.name}에게 {damage}의 피해를 입혔습니다!");
    }

    public void DefenseEffect(BattleSystem defender, BattleSystem _)
    {
        float defenseValue = 5;
        defender.Armor += defenseValue;
        Debug.Log($"[효과 발동] {defender.gameObject.name}이(가) 방어를 {defenseValue} 증가시켰습니다!");
    }

    public void HealEffect(BattleSystem healer, BattleSystem _)
    {
        float healValue = 10;
        healer.curHp += healValue;
        Debug.Log($"[효과 발동] {healer.gameObject.name}이(가) 체력을 {healValue} 회복했습니다!");
    }
}


public class DeckManager : MonoBehaviour
{
    public CardEffects cardEffects;
    public List<Card> deck = new List<Card>(); // 현재 덱
    public List<Card> hand = new List<Card>(); // 현재 손패 리스트
    public int drawCount = 5; // 한 번에 뽑는 카드 수
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

            Debug.Log("JSON 파일에서 카드 데이터를 로드했습니다.");
        }
        else
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {filePath}");
        }
    }

    private void ShowDeck()
    {
        foreach (var card in deck)
        {
            Debug.Log($"카드 이름: {card.cardName}, 유형: {card.cardType}, 에너지 비용: {card.energyCost}, 설명: {card.description}");
        }
    }
    public void RerollCards() //리롤 하는거 구현해야댐
    {

    }
    public void DiscardCard(Card card)
    {
        if (deck.Contains(card))
        {
            deck.Remove(card); // 덱에서 제거
            Debug.Log($"덱에서 카드 {card.cardName}를 지웠습니다.");
        }
        else
        {
            Debug.Log("덱에 없는 카드입니다.");
        }
    }

    public void ShowHand()
    {
        Debug.Log("현재 손패:");
        foreach (var card in hand)
        {
            Debug.Log($"카드 이름: {card.cardName}, 유형: {card.cardType}, 에너지 비용: {card.energyCost}, 설명: {card.description}");
        }
    }

}
