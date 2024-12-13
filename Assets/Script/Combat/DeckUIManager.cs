using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // ī�� UI ������
    public Transform deckWindow; // �� â
    public Transform deckPanel; // �� ǥ�� �г�
    public Transform handPanel; // ���� ǥ�� �г�
    public DeckManager deckManager; // �� �Ŵ���
    public CardEffects cardEffects; // ī�� ȿ�� Ŭ����
    public BattleSystem playerBattleSystem; // �÷��̾� BattleSystem
    public BattleSystem enemyBattleSystem; // ���� BattleSystem

    private bool isPlayerTurn = true; // �� ���� �÷���

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

        if (handPanel != null)
        {
            handPanel.gameObject.SetActive(true);
        }
        ShowHand();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && deckPanel != null && deckWindow != null)
        {
            if (deckWindow.gameObject.activeSelf)
            {
                handPanel.gameObject.SetActive(true);
                deckWindow.gameObject.SetActive(false);
            }
        }
    }

    public void ShowDeck()
    {
        if (handPanel != null)
        {
            handPanel.gameObject.SetActive(false);
        }

        if (deckWindow != null)
        {
            deckWindow.gameObject.SetActive(true);
        }

        if (deckManager == null || deckManager.deck == null || deckManager.deck.Count == 0)
        {
            Debug.LogError("���� ��� �ְų� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        // ���� UI ����
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        // �� ǥ��
        foreach (var card in deckManager.deck)
        {
            GameObject cardObject = Instantiate(cardPrefab, deckPanel);

            Text cardText = cardObject.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = $"{card.cardName}\n{card.cardType}\n{card.description}";
            }

            Image cardImage = cardObject.transform.Find("CardImage").GetComponent<Image>();
            if (cardImage != null)
            {
                Sprite cardSprite = LoadCardImage(card.cardName);
                if (cardSprite != null)
                {
                    cardImage.sprite = cardSprite;
                }
            }
        }

        Debug.Log($"�� {deckManager.deck.Count}���� ī�带 UI�� ǥ���߽��ϴ�.");
    }

    private Sprite LoadCardImage(string imageName)
    {
        string path = $"CardImage/{imageName}";
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
        {
            Debug.LogWarning($"�̹����� ã�� �� �����ϴ�: {imageName}");
        }

        return sprite;
    }

    public void ShowHand()
    {
        // ���� UI ����
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("ShowHand ȣ���");

        if (deckManager == null || deckManager.hand == null || deckManager.hand.Count == 0)
        {
            Debug.LogError("DeckManager.hand�� ��� �ְų� null�Դϴ�!");
            return;
        }

        // ���� ǥ��
        foreach (var card in deckManager.hand)
        {
            GameObject cardObject = Instantiate(cardPrefab, handPanel);

            Text cardText = cardObject.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = $"{card.cardName}\n{card.cardType}\n{card.description}";
            }

            Image cardImage = cardObject.transform.Find("CardImage").GetComponent<Image>();
            if (cardImage != null)
            {
                Sprite cardSprite = LoadCardImage(card.cardName);
                if (cardSprite != null)
                {
                    cardImage.sprite = cardSprite;
                }
            }
        }
    }

    public void RerollCards()
    {
        deckManager.RerollCards();
        ShowHand(); // ���� UI ����
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("�� ����: �÷��̾��� ī�� ȿ���� �����մϴ�.");

        foreach (var card in deckManager.hand)
        {
            if (card.effect != null)
            {
                card.effect.ApplyEffect(playerBattleSystem, new List<Monster> { enemyBattleSystem as Monster });
            }
            else
            {
                Debug.LogWarning($"ī�� {card.cardName}�� ���ǵ� ȿ���� �����ϴ�.");
            }
        }

        deckManager.hand.Clear(); // �� ���� �� ���� �ʱ�ȭ
        ShowHand(); // ���� ����
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        isPlayerTurn = false;
        Debug.Log("������ ���� ���۵˴ϴ�.");

        yield return new WaitForSeconds(1.5f);

        enemyBattleSystem.OnAttack();

        yield return new WaitForSeconds(1.0f);

        Debug.Log("���� �� ����. �÷��̾��� ���� ���۵˴ϴ�.");
        isPlayerTurn = true;

        deckManager.DrawCards(deckManager.drawCount);
        ShowHand();
    }
}
