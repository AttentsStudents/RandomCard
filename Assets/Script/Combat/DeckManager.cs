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
    public List<Card> deck = new List<Card>(); // 현재 덱
    public List<Card> hand = new List<Card>(); // 현재 손패 리스트
    public int drawCount = 5; // 한 번에 뽑는 카드 수 추후 수정 요망

    private void Start()
    {
        LoadCardsFromJson("cards.json");
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

    private void DrawCards(int count)
    {
        for (int i = 0; i < count && deck.Count > 0; i++)
        {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        Debug.Log("카드를 뽑았습니다.");
    }

    private void LoadCardsFromJson(string fileName)
    {
        // 파일 경로 설정
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        try
        {
            // JSON 파일 읽기
            string jsonContent = File.ReadAllText(filePath);

            // JSON 파싱
            CardDataList cardDataList = JsonUtility.FromJson<CardDataList>(jsonContent);

            if (cardDataList == null || cardDataList.cards == null || cardDataList.cards.Count == 0)
            {
                Debug.LogWarning("JSON 데이터가 비어 있거나 유효하지 않습니다.");
                return;
            }

            foreach (var cardData in cardDataList.cards)
            {
                Debug.Log($"카드 데이터 로드: 이름={cardData.cardName}, 타입={cardData.cardType}, 비용={cardData.energyCost}");

                // 카드 타입 파싱
                if (!System.Enum.TryParse(cardData.cardType, out CardType cardType))
                {
                    Debug.LogError($"알 수 없는 카드 타입: {cardData.cardType}");
                    continue;
                }

                // 카드 효과 매핑
                System.Action<BattleSystem, BattleSystem> effect = GetEffectByType(cardType, cardEffects);

                if (effect == null)
                {
                    Debug.LogError($"효과가 없는 카드: {cardData.cardName}");
                    continue;
                }

                // 카드 생성 및 추가
                Card newCard = new Card(cardData.cardName, cardType, cardData.energyCost, cardData.description, effect)
                {
                    imagename = cardData.cardName // 카드 이름을 이미지 이름으로 설정 (필요에 따라 조정 가능)
                };

                deck.Add(newCard);
                Debug.Log($"덱에 추가된 카드: {newCard.cardName}");
            }

            Debug.Log($"총 {deck.Count}장의 카드가 덱에 추가되었습니다.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {e.Message}");
        }

        // Inspector 갱신 (필요시 호출)
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
                Debug.LogError("알 수 없는 카드 타입입니다!");
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
