using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data", order = int.MaxValue)]
public class ItemCard : ScriptableObject
{
    public string card; //카드 이름
    public byte type; // 카드 타입 0 공격, 1 방어, 2 스킬
    public byte figure; //카드 보유 수치(데미지나 실드량)
    public byte remainingTurn; // 효과 지속 턴
    public string description; //카드 설명 txt
    public Sprite sprite; // 카드 이미지

    public string effectclassName; //특수 효과 클래스 이름
    public float[] effectParameters;
}
