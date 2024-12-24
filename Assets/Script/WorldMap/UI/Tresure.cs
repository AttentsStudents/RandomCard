using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap
{
    public class Tresure : MonoBehaviour
    {
        public Transform content;
        Dictionary<int, int> myCardsCount { get; set; }
        void Start()
        {
            CheckMyCards();
            GenerateCards();
        }

        void CheckMyCards()
        {
            myCardsCount = new Dictionary<int, int>();
            foreach (int cardIdx in GameData.playerCards)
            {
                myCardsCount[cardIdx] = myCardsCount.ContainsKey(cardIdx) ?
                    myCardsCount[cardIdx] + 1 : 1;
            }
        }
        void GenerateCards()
        {
            HashSet<int> randomCards = new HashSet<int>();
            while (randomCards.Count < 3)
            {
                int randomCardId = Random.Range(0, CardManager.inst.list.Count);
                if (!randomCards.Contains(randomCardId)) randomCards.Add(randomCardId);
            }
            foreach (int cardIdx in randomCards)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/TresureCard"), content);
                TresureCard card = obj.GetComponent<TresureCard>();
                card.data = CardManager.inst.list[cardIdx].data;
                card.count = myCardsCount.ContainsKey(cardIdx) ? myCardsCount[cardIdx] : 0;
                card.ClickAction = () =>
                {
                    Destroy(gameObject);
                    GameData.ClearTargetNode();
                };
            }
        }
    }
}

