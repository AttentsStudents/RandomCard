using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<ItemCard> cardDataScriptableObjects; // 여러 ScriptableObject를 참조할 리스트
    public List<Card> deck = new List<Card>(); // 현재 덱
    public List<Card> hand = new List<Card>(); // 현재 손패 리스트
    public int drawCount = 3; // 한 번에 뽑는 카드 수

    private void Start()
    {
        LoadCardsFromScriptableObjects(); // ScriptableObject에서 데이터 로드
        if (deck == null || deck.Count == 0)
        {
            Debug.LogError("덱이 비어 있습니다. 카드 데이터를 확인하세요.");
        }
        else
        {
            Debug.Log($"덱에 총 {deck.Count}장의 카드가 있습니다.");
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
        Debug.Log("카드를 리롤했습니다.");
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
        Debug.Log("덱을 섞었습니다.");
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count && deck.Count > 0; i++)
        {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        Debug.Log("카드를 뽑았습니다.");
    }


    private void LoadCardsFromScriptableObjects()
    {
        if (cardDataScriptableObjects == null || cardDataScriptableObjects.Count == 0)
        {
            Debug.LogError("ScriptableObject 데이터가 비어 있거나 유효하지 않습니다.");
            return;
        }

        foreach (var itemCard in cardDataScriptableObjects)
        {
            Debug.Log($"카드 데이터 로드: 이름={itemCard.card}, 타입={itemCard.type}, 설명={itemCard.description}");

            // 카드 타입 매핑
            CardType cardType = GetCardTypeFromByte(itemCard.type);

            // 카드 생성 및 추가
            Card newCard = new Card(
                itemCard.card,
                cardType,
                0, // 에너지 비용 (ItemCard에 정의되지 않음, 필요하다면 ScriptableObject에 추가)
                itemCard.description,
                null // 현재 효과는 정의되지 않음
            )
            {
                sprite = itemCard.sprite // ScriptableObject에서 스프라이트 설정
            };

            deck.Add(newCard);
            //Debug.Log($"덱에 추가된 카드: {newCard.cardName}");
        }

        Debug.Log($"총 {deck.Count}장의 카드가 덱에 추가되었습니다.");
    }

    private CardType GetCardTypeFromByte(byte type)
    {
        // byte 값을 CardType enum으로 매핑
        switch (type)
        {
            case 0:
                return CardType.Attack;
            case 1:
                return CardType.Defense;
            case 2:
                return CardType.Skill;
            default:
                Debug.LogError($"알 수 없는 카드 타입: {type}");
                return CardType.Attack; // 기본값 설정
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
    public string cardName; // 카드 이름
    public CardType cardType; // 카드 타입
    public int energyCost; // 에너지 비용
    public string description; // 카드 설명
    public Sprite sprite; // 카드 이미지
    public System.Action<BattleSystem, BattleSystem> effect; // 카드 효과 (추후 구현 가능)

    public Card(string name, CardType type, int cost, string desc, System.Action<BattleSystem, BattleSystem> cardEffect)
    {
        cardName = name;
        cardType = type;
        energyCost = cost;
        description = desc;
        effect = cardEffect;
    }
}
