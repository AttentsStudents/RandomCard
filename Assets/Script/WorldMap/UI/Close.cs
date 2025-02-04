using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WorldMap
{
    
    public class Close : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent ClickEvent;
        public void OnPointerClick(PointerEventData eventData)
        {
            ClickEvent?.Invoke();
        }
    }
}
