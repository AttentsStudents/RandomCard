using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data", order = int.MaxValue)]
public class ItemCard : ScriptableObject
{
    public string card; //ī�� �̸�
    public byte type; // ī�� Ÿ�� 0 ����, 1 ���, 2 ��ų
    public byte figure; //ī�� ���� ��ġ(�������� �ǵ差)
    public byte remainingTurn; // ȿ�� ���� ��
    public string description; //ī�� ���� txt
    public Sprite sprite; // ī�� �̹���

    public string effectclassName; //Ư�� ȿ�� Ŭ���� �̸�
    public float[] effectParameters;
}
