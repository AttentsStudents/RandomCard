using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CheonJiWoon
{
    public class TresureCard : Card, IPointerClickHandler, IClickAction, IPointerEnterHandler, IPointerExitHandler
    {
        
        public UnityAction ClickAction { get; set; }
        float defaultAlpha = 0.85f;
        float enterAlpha = 1.0f;

        void Start()
        {
            ChangeImageAlpha(defaultAlpha);
            image.sprite = data.sprite;
        }


        public void OnPointerEnter(PointerEventData eventData) => ChangeImageAlpha(enterAlpha);

        public void OnPointerExit(PointerEventData eventData) => ChangeImageAlpha(defaultAlpha);

        public void OnPointerClick(PointerEventData eventData)
        {
            GetCard();
            ClickAction?.Invoke();
        }


        void GetCard() => GameData.AddCard(data.type);

        void ChangeImageAlpha(float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }

}
