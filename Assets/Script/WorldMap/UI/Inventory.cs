using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace WorldMap
{
    public class Inventory : MonoBehaviour
    {
        Dictionary<byte, int> inventory;
        public Transform content;
        void Start()
        {
            inventory = new Dictionary<byte, int>();

            foreach (byte cardIdx in GameData.playerCards)
            {
                if (inventory.ContainsKey(cardIdx)) inventory[cardIdx]++;
                else inventory.Add(cardIdx, 1);
            }
            foreach (var item in inventory)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/InventoryCard"), content);
                InventoryCard card = obj.GetComponent<InventoryCard>();
                if (card != null)
                {
                    card.count = item.Value;
                    card.data = CardManager.inst.idxToData[item.Key];
                }
            }
            GameData.AddCardAction = (byte cardIdx) => OnAddCard(cardIdx);
        }

        public void OnAddCard(byte cardIdx)
        {
            if (inventory.ContainsKey(cardIdx))
            {
                inventory[cardIdx]++;
                InventoryCard[] cardList = content.GetComponentsInChildren<InventoryCard>();
                foreach (InventoryCard card in cardList)
                {
                    if (card.data == CardManager.inst.idxToData[cardIdx])
                    {
                        card.count = inventory[cardIdx];
                        break;
                    }
                }
            }
            else
            {
                inventory.Add(cardIdx, 1);
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/InventoryCard"), content);
                InventoryCard card = obj.GetComponent<InventoryCard>();
                card.data = CardManager.inst.list[cardIdx].data;
                card.count = inventory[cardIdx];
            }
        }
    }
}

