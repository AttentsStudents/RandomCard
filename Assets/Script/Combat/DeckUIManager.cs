using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // ī�� UI�̹���
    public Transform deckPanel;  // �� ǥ��
    public Transform handPanel;  // ���� ǥ��
    public DeckManager deckManager;

    // �� UI ����
    public void ShowDeck()
    {
        // ���� UI ����
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        // ���� ��� ī�带 UI�� ����
        foreach (var card in deckManager.deck)
        {
            GameObject cardObject = Instantiate(cardPrefab, deckPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();

            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
        }
    }

    // ���� UI ����
    public void ShowHand()
    {
        // ���� UI ����
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        // ������ ��� ī�带 UI�� ����
        foreach (var card in deckManager.hand)
        {
            GameObject cardObject = Instantiate(cardPrefab, handPanel);
            Text cardText = cardObject.GetComponentInChildren<Text>();

            cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
        }
    }

    // ī�� �̱� ��ư
    public void RerollCards()
    {
        deckManager.RerollCards(); // ������ ī�带 ����
        ShowHand();              // ���� ī�带 ���� UI�� ǥ��
    }
}
