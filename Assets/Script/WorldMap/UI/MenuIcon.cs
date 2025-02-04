using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WorldMap
{
    public class MenuIcon : ImageProperty, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IClickAction
    {
        public GameObject target;
        public UnityAction ClickAction { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (target.activeSelf) return;

            Color originColor = image.color;
            originColor.a = SceneData.iconOnAlpha;
            image.color = originColor;

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (target.activeSelf) return;

            Color originColor = image.color;
            originColor.a = SceneData.iconOffAlpha;
            image.color = originColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ClickAction?.Invoke();
        }
    }
}

