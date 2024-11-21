using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data", order = int.MaxValue)]
public class ItemCard : ScriptableObject
{
    public string card;
    public byte type;
    public string description;
    public Sprite sprite;
}
