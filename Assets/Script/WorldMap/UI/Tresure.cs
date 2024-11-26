using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Tresure : MonoBehaviour
    {
        public Transform content;
        void Start()
        {
            GenerateCards();
        }

        void GenerateCards()
        {
            HashSet<int> randomCards = new HashSet<int>();
            while (randomCards.Count < 3)
            {
                int randomCardId = Random.Range(0, CardManager.instance.cards.Length);
                if (!randomCards.Contains(randomCardId))
                {
                    randomCards.Add(randomCardId);
                }
            }
            foreach (int cardIdx in randomCards)
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/TresureCard"), content);
                TresureCard card = obj.GetComponent<TresureCard>();
                if(card != null )
                {
                    card.data = CardManager.instance.cards[cardIdx];
                    card.ClickAction = () => Destroy(gameObject);
                }
            }
        }
    }
}

