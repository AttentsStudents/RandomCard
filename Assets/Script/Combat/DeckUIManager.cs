using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // ī�� UI ������
    public Transform deckPanel;  // �� ǥ�� �г�
    public Transform handPanel;  // ���� ǥ�� �г�
    public DeckManager deckManager;
    public CardEffects cardEffects; // ī�� ȿ�� Ŭ����
    public BattleSystem playerBattleSystem; // �÷��̾� BattleSystem
    public BattleSystem enemyBattleSystem; // ���� BattleSystem

    private bool isPlayerTurn = true; // �� ���� �÷���

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
        handPanel.gameObject.SetActive(true);
        ShowHand();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (deckPanel.gameObject != null && deckPanel.gameObject.activeSelf)
            {
                // �г� ��Ȱ��ȭ
                deckPanel.gameObject.SetActive(false);
                handPanel.gameObject.SetActive(true);
            }
        }
    }

    public void ShowDeck()
    {
        handPanel.gameObject.SetActive(false);
        deckPanel.gameObject.SetActive(true);
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
            //������ ����
            GameObject cardObject = Instantiate(cardPrefab, deckPanel);

            Text cardText = cardObject.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
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
        //UpdateUI(deckManager.deck, deckPanel);
    }

    private Sprite LoadCardImage(string imageName)
    {
        string path = $"CardImage/{imageName}";
        Debug.Log($"�ε� �õ� ���: {path}");
        Sprite sprite = Resources.Load<Sprite>(path);
        
        if (sprite == null)
        {
            Debug.LogWarning($"�̹����� ã�� �� �����ϴ�: {imageName}");
        }
        else
        {
            Debug.Log($"�̹��� �ε� ����: {imageName}");
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

        if (deckManager == null)
        {
            Debug.LogError("DeckManager�� null�Դϴ�!");
            return;
        }

        if (deckManager.hand == null || deckManager.hand.Count == 0)
        {
            Debug.LogError("DeckManager.hand�� ��� �ְų� null�Դϴ�!");
            return;
        }

        // ������ �� ī�� �����͸� UI�� ���
        foreach (var card in deckManager.hand)
        {
            // 1. ī�� ������ ����
            GameObject cardObject = Instantiate(cardPrefab, handPanel);

            // 2. �ؽ�Ʈ ������Ʈ�� ī�� ���� �Ҵ�
            Text cardText = cardObject.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
            }

            // 3. �̹��� ������Ʈ�� ī�� ��������Ʈ �Ҵ�
            Image cardImage = cardObject.transform.Find("CardImage")?.GetComponent<Image>();
            if (cardImage != null)
            {
                Sprite cardSprite = LoadCardImage(card.cardName);
                if (cardSprite != null)
                {
                    cardImage.sprite = cardSprite;
                    Debug.Log($"Hand ī�� �̹��� �ε� ����: {card.cardName}");
                }
                else
                {
                    Debug.LogError($"Hand ī�� �̹��� �ε� ����: {card.cardName}");
                }
            }
            else
            {
                Debug.LogError("Hand �гο��� CardImage ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }

    public void RerollCards()
    {
        deckManager.RerollCards();
        ShowDeck(); // �� UI ����
        ShowHand(); // ���� UI ����
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return; // �÷��̾� ���� �ƴϸ� �������� ����

        Debug.Log("�� ����: �÷��̾��� ī�� ȿ���� �����մϴ�.");

        // �ڵ� �г��� ��� ī�� ȿ���� ���Ϳ��� ����
        foreach (var card in deckManager.hand)
        {
            switch (card.cardType)
            {
                case CardType.Attack:
                    cardEffects.AttackEffect(playerBattleSystem, enemyBattleSystem);
                    break;
                case CardType.Defense:
                    cardEffects.DefenseEffect(playerBattleSystem, null);
                    break;
                case CardType.Skill:
                    cardEffects.HealEffect(playerBattleSystem, null);
                    break;
                default:
                    Debug.LogWarning($"�� �� ���� ī�� Ÿ��: {card.cardType}");
                    break;
            }
        }

        // �� ���� �� �ڵ带 ���� ���� ������ ��ȯ
        deckManager.hand.Clear();
        ShowHand(); // �ڵ� �г� �ʱ�ȭ
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        isPlayerTurn = false; // ���� �� ����
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
