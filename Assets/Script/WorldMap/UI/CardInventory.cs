using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class CardInventory : MonoBehaviour
    {
        Dictionary<int, int> inventory;
        public Transform content;
        void Start()
        {
            inventory = new Dictionary<int, int>();

            foreach (int cardType in GameData.cards)
            {
                if (inventory.ContainsKey(cardType))
                {
                    inventory[cardType]++;
                }
                else inventory.Add(cardType, 1);
            }
            foreach (var item in inventory)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/CardInvenItem"), content);
                CardInvenItem card = obj.GetComponent<CardInvenItem>();
                if(card != null)
                {
                    card.count = item.Value;
                    card.data = CardManager.instance.cardTypeToData[item.Key];
                }
            }
        }
    }
}

