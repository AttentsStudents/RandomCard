using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public abstract class DragCard : Card, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static DragCard draggingCard {get; set;}
        Vector3 originPos;
        public void OnBeginDrag(PointerEventData eventData)
        {
            draggingCard = this;
            originPos = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            draggingCard = null;
            transform.position = originPos;
        }
    }
}

