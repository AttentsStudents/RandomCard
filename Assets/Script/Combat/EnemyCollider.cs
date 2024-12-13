using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

interface IDropObserb<T> : IDropHandler
{
    public UnityAction<T> DropObserb { get; set; }
}

namespace Combat
{
    public class EnemyCollider : MonoBehaviour, IDropObserb<IBattleStat>, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject target { get; set; }
        public GameObject Arrow;
        public UnityAction<IBattleStat> DropObserb { get; set; }

        void Start()
        {
            Vector3 scPos = Camera.main.WorldToScreenPoint(target.transform.position);
            scPos.y += 50;
            transform.position = scPos;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("드롭");
            if(eventData.pointerDrag.TryGetComponent<Card>(out Card card)){
                card.Active();
                Debug.Log("발동!!");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(DragCard.draggingCard != null)
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

