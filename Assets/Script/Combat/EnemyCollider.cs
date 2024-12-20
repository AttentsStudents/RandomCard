using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Combat
{
    public class EnemyCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        public GameObject target { get; set; }
        public GameObject Arrow;

        void Update()
        {
            Vector3 scPos = Camera.main.WorldToScreenPoint(target.transform.position);
            scPos.y += 50;
            transform.position = scPos;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent<Card>(out Card card) 
                && target.TryGetComponent(out BattleSystem battle))
            {
                if(card.Active(new List<BattleSystem>() { battle }))
                {
                    card.gameObject.transform.localPosition = Vector3.zero;
                    CardPool.inst?.Push(card.gameObject);
                    DragCard.draggingCard = null;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (DragCard.draggingCard != null)
            {
                Arrow.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Arrow.SetActive(false);
        }
    }
}

