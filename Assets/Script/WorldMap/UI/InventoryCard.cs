using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldMap
{
    public class InventoryCard : Card
    {
        int _count;
        public int count { 
            get => _count; 
            set 
            {
                cardCount.text = $"°¹¼ö: {value}";
                _count = value;
            }
        }

        public TMPro.TMP_Text cardName;
        public TMPro.TMP_Text description;
        public TMPro.TMP_Text cardCount;

        void Start()
        {
            LoadData();
        }

        void LoadData()
        {
            image.sprite = data.sprite;
            cardName.text = data.card;
            description.text = data.description;
        }
    }

}
