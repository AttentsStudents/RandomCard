using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CheonJiWoon
{
    public class CardInvenItem : MonoBehaviour
    {
        public ItemCard data { get; set; }
        public int count { get; set; }
        public Image image;

        public TMPro.TMP_Text cardName;
        public TMPro.TMP_Text description;

        void Start()
        {
            InitCard();
        }

        void InitCard()
        {
            image.sprite = data.sprite;
            cardName.text = data.card;
            description.text = data.description;
        }
    }

}
