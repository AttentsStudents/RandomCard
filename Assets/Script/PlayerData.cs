using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/Player Data", order = int.MaxValue)]
public class PlayerData : ScriptableObject
{
    public float maxHP = 100.0f;
    public float armor = 10.0f;
    public float attack = 15.0f;
}
