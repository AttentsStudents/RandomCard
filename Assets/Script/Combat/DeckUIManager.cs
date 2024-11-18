using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // 카드 UI이미지
    public Transform deckPanel;  // 덱 표시
    public Transform handPanel;  // 손패 표시
    public DeckManager deckManager;

    // 덱 UI 생성
    public void ShowDeck()
    {
        // 기존 UI 제거
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        // 덱의 모든 카드를 UI로 생성
        foreach (var card in deckManager.deck)
        {
            GameObject cardObject = Instantiate(cardPrefab, deckPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();

            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
        }
    }

    // 손패 UI 생성
    public void ShowHand()
    {
        // 기존 UI 제거
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        // 손패의 모든 카드를 UI로 생성
        foreach (var card in deckManager.hand)
        {
            GameObject cardObject = Instantiate(cardPrefab, handPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();

            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
        }
    }

    // 카드 뽑기 버튼
    public void RerollCards()
    {
        deckManager.RerollCards(); // 덱에서 카드를 뽑음
        ShowHand();              // 뽑은 카드를 손패 UI로 표시
    }
}
