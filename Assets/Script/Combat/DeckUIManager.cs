using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // 카드 UI 프리팹
    public Transform deckPanel;  // 덱 표시 패널
    public Transform handPanel;  // 손패 표시 패널
    public DeckManager deckManager;

    private Dictionary<string, Sprite> cardImageCache = new Dictionary<string, Sprite>();

    private void Start()
    {
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManager>();
            if (deckManager == null)
            {
                Debug.LogError("DeckManager를 찾을 수 없습니다.");
            }
        }
        handPanel.gameObject.SetActive(true);
        //ShowDeck();
    }


    public void ShowDeck()
    {
        deckPanel.gameObject.SetActive(true);
        if (deckManager == null || deckManager.deck == null || deckManager.deck.Count == 0)
        {
            Debug.LogError("덱이 비어 있거나 초기화되지 않았습니다.");
            return;
        }

        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (var card in deckManager.deck)
        {
            GameObject cardObject = Instantiate(cardPrefab, deckPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();
            Image cardImage = cardObject.transform.Find("CardImage").GetComponent<Image>();
            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
            Sprite cardSprite = LoadCardImage(card.cardName);
            if (cardSprite != null)
            {
                cardImage.sprite = cardSprite;
            }
        }

        Debug.Log($"총 {deckManager.deck.Count}장의 카드를 UI로 표시했습니다.");
        //UpdateUI(deckManager.deck, deckPanel);
    }

    private Sprite LoadCardImage(string imageName)
    {
        //Sprite sprite = Resources.Load<Sprite>($"CardImage/{imageName}");
        string path = $"CardImage/{imageName}";
        Debug.Log($"로드 시도 경로: {path}");
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning($"이미지를 찾을 수 없습니다: {imageName}");
        }
        else
        {
            Debug.Log($"이미지 로드 성공: {imageName}");
        }

        return sprite;
    }


    public void ShowHand()
    {
        if (deckManager.hand == null || deckManager.hand.Count == 0)
        {
            Debug.LogWarning("손패가 비어 있습니다.");
            return;
        }

        UpdateUI(deckManager.hand, handPanel);
    }

    private void UpdateUI(List<Card> cards, Transform parentPanel)
    {
        foreach (Transform child in parentPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (var card in cards)
        {
            GameObject cardObject = Instantiate(cardPrefab, parentPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();
            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";

            Image cardImage = cardObject.transform.Find("CardImage").GetComponent<Image>();
            Sprite cardSprite = LoadCardImage(card.cardName);
            if (cardSprite != null)
            {
                cardImage.sprite = cardSprite;
            }
        }
    }


    public void RerollCards()
    {
        deckManager.RerollCards();
        ShowDeck(); // 덱 UI 갱신
        ShowHand(); // 손패 UI 갱신
    }
}
