using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : SingleTon<CardManager>
{
    public List<Card> list;
    public Dictionary<byte, ItemCard> idxToData { get; private set; }
    public Dictionary<ItemCard, byte> dataToIdx { get; private set; }
    void Awake()
    {
        Init();
        idxToData = new Dictionary<byte, ItemCard>();
        dataToIdx = new Dictionary<ItemCard, byte>();
        for (byte i = 0; i < list.Count; i++)
        {
            idxToData[i] = list[i].data;
            dataToIdx[list[i].data] = i;
        }
    }
}
