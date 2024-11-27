using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DeckUIManager : MonoBehaviour
{
    public GameObject cardPrefab; // 카드 UI 프리팹
    public Transform deckPanel;  // 덱 표시 패널
    public Transform handPanel;  // 손패 표시 패널
    public DeckManager deckManager;
    public CardEffects cardEffects; // 카드 효과 클래스
    public BattleSystem playerBattleSystem; // 플레이어 BattleSystem
    public BattleSystem enemyBattleSystem; // 몬스터 BattleSystem

    private bool isPlayerTurn = true; // 턴 관리 플래그

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
        ShowHand();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (deckPanel.gameObject != null && deckPanel.gameObject.activeSelf)
            {
                // 패널 비활성화
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
            Debug.LogError("덱이 비어 있거나 초기화되지 않았습니다.");
            return;
        }

        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (var card in deckManager.deck)
        {
            //프리팹 생성
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


        Debug.Log($"총 {deckManager.deck.Count}장의 카드를 UI로 표시했습니다.");
        //UpdateUI(deckManager.deck, deckPanel);
    }

    private Sprite LoadCardImage(string imageName)
    {
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
        // 기존 UI 제거
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("ShowHand 호출됨");

        if (deckManager == null)
        {
            Debug.LogError("DeckManager가 null입니다!");
            return;
        }

        if (deckManager.hand == null || deckManager.hand.Count == 0)
        {
            Debug.LogError("DeckManager.hand가 비어 있거나 null입니다!");
            return;
        }

        // 손패의 각 카드 데이터를 UI에 출력
        foreach (var card in deckManager.hand)
        {
            // 1. 카드 프리팹 생성
            GameObject cardObject = Instantiate(cardPrefab, handPanel);

            // 2. 텍스트 컴포넌트에 카드 정보 할당
            Text cardText = cardObject.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = $"{card.cardName}\n{card.cardType}\nCost: {card.energyCost}\n{card.description}";
            }

            // 3. 이미지 컴포넌트에 카드 스프라이트 할당
            Image cardImage = cardObject.transform.Find("CardImage")?.GetComponent<Image>();
            if (cardImage != null)
            {
                Sprite cardSprite = LoadCardImage(card.cardName);
                if (cardSprite != null)
                {
                    cardImage.sprite = cardSprite;
                    Debug.Log($"Hand 카드 이미지 로드 성공: {card.cardName}");
                }
                else
                {
                    Debug.LogError($"Hand 카드 이미지 로드 실패: {card.cardName}");
                }
            }
            else
            {
                Debug.LogError("Hand 패널에서 CardImage 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    public void RerollCards()
    {
        deckManager.RerollCards();
        ShowDeck(); // 덱 UI 갱신
        ShowHand(); // 손패 UI 갱신
    }

    public void EndTurn()
    {
        if (!isPlayerTurn) return; // 플레이어 턴이 아니면 실행하지 않음

        Debug.Log("턴 종료: 플레이어의 카드 효과를 적용합니다.");

        // 핸드 패널의 모든 카드 효과를 몬스터에게 적용
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
                    Debug.LogWarning($"알 수 없는 카드 타입: {card.cardType}");
                    break;
            }
        }

        // 턴 종료 후 핸드를 비우고 다음 턴으로 전환
        deckManager.hand.Clear();
        ShowHand(); // 핸드 패널 초기화
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        isPlayerTurn = false; // 몬스터 턴 시작
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
