using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace WorldMap
{
    public class Inventory : MonoBehaviour
    {
        Dictionary<int, int> inventory;
        public Transform content;
        void Start()
        {
            inventory = new Dictionary<int, int>();

            foreach (int cardType in GameData.cards)
            {
                if (inventory.ContainsKey(cardType)) inventory[cardType]++;
                else inventory.Add(cardType, 1);
            }
            foreach (var item in inventory)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/InventoryCard"), content);
                InventoryCard card = obj.GetComponent<InventoryCard>();
                if (card != null)
                {
                    card.count = item.Value;
                    card.data = CardManager.instance.cardTypeToData[item.Key];
                }
            }
            GameData.AddCardAction = (byte cardType) => OnAddCard(cardType);
        }

        public void OnAddCard(byte cardType)
        {
            if (inventory.ContainsKey(cardType))
            {
                inventory[cardType]++;
                InventoryCard[] cardList = content.GetComponentsInChildren<InventoryCard>();
                foreach (InventoryCard card in cardList)
                {
                    if (card.data.type == cardType)
                    {
                        card.count = inventory[cardType];
                        break;
                    }
                }
            }
            else
            {
                inventory.Add(cardType, 1);
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/InventoryCard"), content);
                InventoryCard card = obj.GetComponent<InventoryCard>();
                card.data = CardManager.instance.cardTypeToData[cardType];
                card.count = inventory[cardType];
            }
        }
    }
}

