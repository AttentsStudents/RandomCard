using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Tresure : MonoBehaviour
    {
        public Transform content;
        Dictionary<int, int> myCardsTypeCount { get; set; }
        void Start()
        {
            CheckMyCards();
            GenerateCards();
        }

        void CheckMyCards()
        {
            myCardsTypeCount = new Dictionary<int, int>();
            foreach (int cardType in GameData.cards)
            {
                myCardsTypeCount[cardType] = myCardsTypeCount.ContainsKey(cardType) ?
                    myCardsTypeCount[cardType] + 1 : 1;
            }
        }
        void GenerateCards()
        {
            HashSet<int> randomCards = new HashSet<int>();
            while (randomCards.Count < 3)
            {
                int randomCardId = Random.Range(0, CardManager.instance.cards.Length);
                if (!randomCards.Contains(randomCardId)) randomCards.Add(randomCardId);
            }
            foreach (int cardIdx in randomCards)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/TresureCard"), content);
                TresureCard card = obj.GetComponent<TresureCard>();
                card.data = CardManager.instance.cards[cardIdx];
                card.count = myCardsTypeCount.ContainsKey(card.data.type) ? myCardsTypeCount[card.data.type] : 0;
                card.ClickAction = () =>
                {
                    Destroy(gameObject);
                    GameData.SaveData();
                };
            }
        }
    }
}

