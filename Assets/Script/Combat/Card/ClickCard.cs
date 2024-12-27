using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public abstract class ClickCard : Card, IPointerDownHandler
    {
        public abstract void OnPointerDown(PointerEventData eventData);
    }
}

