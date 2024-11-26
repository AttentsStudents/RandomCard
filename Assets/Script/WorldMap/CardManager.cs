using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : SingleTon<CardManager>
{
    public ItemCard[] cards;
    public Dictionary<int, ItemCard> cardTypeToData { get; set; }
    void Awake()
    {
        Init();
        cardTypeToData = new Dictionary<int, ItemCard>();
        foreach (ItemCard data in cards)
        {
            cardTypeToData.Add(data.type, data);
        }
    }
}
