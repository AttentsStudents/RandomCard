using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // ī�� UI ������
    public Transform deckPanel;  // �� ǥ�� �г�
    public Transform handPanel;  // ���� ǥ�� �г�
    public DeckManager deckManager;

    private Dictionary<string, Sprite> cardImageCache = new Dictionary<string, Sprite>();

    private void Start()
    {
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManager>();
            if (deckManager == null)
            {
                Debug.LogError("DeckManager�� ã�� �� �����ϴ�.");
            }
        }

        ShowDeck();
    }


    public void ShowDeck()
    {
        if (deckManager == null || deckManager.deck == null || deckManager.deck.Count == 0)
        {
            Debug.LogError("���� ��� �ְų� �ʱ�ȭ���� �ʾҽ��ϴ�.");
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

            // ī�� �ؽ�Ʈ ����
            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";

            // ī�� �̹��� ����
            Sprite cardSprite = LoadCardImage(card.cardName);
            if (cardSprite != null)
            {
                cardImage.sprite = cardSprite;
            }
        }

        Debug.Log($"�� {deckManager.deck.Count}���� ī�带 UI�� ǥ���߽��ϴ�.");
        //UpdateUI(deckManager.deck, deckPanel);
    }

    private Sprite LoadCardImage(string imageName)
    {
        if (cardImageCache.ContainsKey(imageName))
        {
            return cardImageCache[imageName];
        }

        Sprite sprite = Resources.Load<Sprite>($"CardImage/{imageName}");
        if (sprite != null)
        {
            cardImageCache[imageName] = sprite;
        }
        else
        {
            Debug.LogWarning($"�̹����� ã�� �� �����ϴ�: {imageName}");
        }

        return sprite;
    }

    public void ShowHand()
    {
        if (deckManager.hand == null || deckManager.hand.Count == 0)
        {
            Debug.LogWarning("���а� ��� �ֽ��ϴ�.");
            return;
        }

        UpdateUI(deckManager.hand, handPanel);
    }

    private void UpdateUI(List<Card> cards, Transform parentPanel)
    {
        // ���� UI ����
        foreach (Transform child in parentPanel)
        {
            Destroy(child.gameObject);
        }

        // ī�� UI ����
        foreach (var card in cards)
        {
            GameObject cardObject = Instantiate(cardPrefab, parentPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();
            Image cardImage = cardObject.transform.Find("CardImage").GetComponent<Image>();

            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";

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
        ShowDeck(); // �� UI ����
        ShowHand(); // ���� UI ����
    }
}