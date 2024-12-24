using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data", order = int.MaxValue)]
public class ItemCard : ScriptableObject
{
    public string card; //카드 이름
    public string description; //카드 설명 txt
    public Sprite sprite; // 카드 이미지
}
