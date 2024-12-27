using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsFrame : MonoBehaviour
{
    public Transform cards;
    public Transform waitCards;
    public CardPool cardPool;
    public int handCardCount = 6;
    public int turnCardCount = 2;

    void Awake()
    {
        List<byte> list = new List<byte>(GameData.playerCards);
        for (int i = 0; i < list.Count; i++)
        {
            byte temp = list[i];
            int rIdx = Random.Range(0, list.Count);
            list[i] = list[rIdx];
            list[rIdx] = temp;
        }

        GetCardFromList(list);
    }

    public void OnCardToHands()
    {
        for (int i = 0; i < turnCardCount; i++)
        {
            if (cards.childCount >= handCardCount) return;
            if (waitCards.childCount == 0)
            {
                while(cardPool.count > 0)
                {
                    cardPool.Pop().transform.SetParent(waitCards);
                }
            }
            waitCards.GetChild(0).SetParent(cards);
        }
    }

    public void GetCardFromList(List<byte> list)
    {
        int count = 0;
        while (list.Count > 0)
        {
            Instantiate(CardManager.inst.list[list[0]].gameObject, count < 6 ? cards : waitCards);
            list.RemoveAt(0);
            count++;
        }
    }
}
