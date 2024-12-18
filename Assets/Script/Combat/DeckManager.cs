using System.Collections.Generic;
using UnityEngine;
using static CardEffects;

public class DeckManager : MonoBehaviour
{
    public List<ItemCard> cardDataScriptableObjects; // ScriptableObject 리스트
    public List<Card> deck = new List<Card>(); // 현재 덱
    public List<Card> hand = new List<Card>(); // 현재 손패 리스트
    public int drawCount = 3; // 드로우 카드 수
    public static int Atp, Dep, Sp = 0; // 카드 카운트

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
        Atp = 0;
        Dep = 0;
        Sp = 0;
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
            Debug.LogError("ScriptableObject 데이터가 비어 있거나 유효하지 않습니다.");
            return;
        }

        foreach (var itemCard in cardDataScriptableObjects)
        {

            // 카드 타입 매핑
            CardType cardType = GetCardTypeFromByte(itemCard.type);

            // 카드 생성
            Card newCard = new Card(
                itemCard.card,
                cardType,
                itemCard.description,
                null // 현재 효과는 정의되지 않음
            )
            {
                effectValue = itemCard.figure, // ScriptableObject의 `figure` 값으로 초기화
                sprite = itemCard.sprite       // ScriptableObject의 스프라이트 설정
            };

            // 덱에 추가
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
            _ => throw new System.Exception($"알 수 없는 카드 타입: {type}")
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
    public string cardName;       // 카드 이름
    public CardType cardType;     // 카드 타입
    public string description;    // 카드 설명
    public ICardEffect effect;    // 카드 효과 클래스
    public Sprite sprite;         // 카드 이미지
    public float effectValue;     // 카드 효과 값 (예: 공격력, 방어력 등)

    public Card(string name, CardType type, string desc, ICardEffect cardEffect)
    {
        cardName = name;
        cardType = type;
        description = desc;
        effect = cardEffect;
    }
}
