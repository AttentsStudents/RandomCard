using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tresure : MonoBehaviour
{
    void Start()
    {
        GenerateCards();
    }

    void GenerateCards()
    {
        HashSet<int> check = new HashSet<int>();
        while (check.Count < 3)
        {
            int randomCardId = Random.Range(0, 10);
            if (!check.Contains(randomCardId))
            {
                check.Add(randomCardId);
            }
        }
        foreach (int cardId in check)
        {
        }
    }
}
