using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // 카드 UI 프리팹
    public Transform deckWindow; // 덱 창
    public Transform deckPanel; // 덱 표시 패널
    public Transform handPanel; // 손패 표시 패널
    public DeckManager deckManager; // 덱 매니저
    public CardEffects cardEffects; // 카드 효과 클래스
    public BattleSystem playerBattleSystem; // 플레이어 BattleSystem
    public BattleSystem enemyBattleSystem; // 몬스터 BattleSystem

    private bool isPlayerTurn = true; // 턴 관리 플래그

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
            Debug.LogError("덱이 비어 있거나 초기화되지 않았습니다.");
            return;
        }

        // 기존 UI 제거
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        // 덱 표시
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

        Debug.Log($"총 {deckManager.deck.Count}장의 카드를 UI로 표시했습니다.");
    }

    private Sprite LoadCardImage(string imageName)
    {
        string path = $"CardImage/{imageName}";
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
        {
            Debug.LogWarning($"이미지를 찾을 수 없습니다: {imageName}");
        }

        return sprite;
    }

    public void ShowHand()
    {
        // 기존 UI 제거
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("ShowHand 호출됨");

        if (deckManager == null || deckManager.hand == null || deckManager.hand.Count == 0)
        {
            Debug.LogError("DeckManager.hand가 비어 있거나 null입니다!");
            return;
        }

        // 손패 표시
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
        ShowHand(); // 손패 UI 갱신
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        Debug.Log("턴 종료: 플레이어의 카드 효과를 적용합니다.");

        foreach (var card in deckManager.hand)
        {
            if (card.effect != null)
            {
                card.effect.ApplyEffect(playerBattleSystem, new List<Monster> { enemyBattleSystem as Monster });
            }
            else
            {
                Debug.LogWarning($"카드 {card.cardName}에 정의된 효과가 없습니다.");
            }
        }

        deckManager.hand.Clear(); // 턴 종료 후 손패 초기화
        ShowHand(); // 손패 갱신
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        isPlayerTurn = false;
        Debug.Log("몬스터의 턴이 시작됩니다.");

        yield return new WaitForSeconds(1.5f);

        enemyBattleSystem.OnAttack();

        yield return new WaitForSeconds(1.0f);

        Debug.Log("몬스터 턴 종료. 플레이어의 턴이 시작됩니다.");
        isPlayerTurn = true;

        deckManager.DrawCards(deckManager.drawCount);
        ShowHand();
    }
}
