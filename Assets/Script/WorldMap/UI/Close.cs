using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CheonJiWoon
{
    
    public class Close : MonoBehaviour, IPointerDownHandler, ICloseAction
    {
        public UnityAction CloseAction { get; set; }
        public void OnPointerDown(PointerEventData eventData)
        {
            CloseAction?.Invoke();
        }
    }
}
