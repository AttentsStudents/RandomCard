using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class MapIcon : ImageProperty
    {
        public Transform target { get; set; }
        RectTransform parent;
        RectTransform rectTransform;
        void Start()
        {
            Init();
            UpdateTransform();
        }
        protected void Init()
        {
            parent = transform.parent as RectTransform;
            rectTransform = transform as RectTransform;
        }

        protected void UpdateTransform()
        {
            Vector3 viewPos = Map.instance.mapCamera.WorldToViewportPoint(target.position);
            viewPos.x = viewPos.x * parent.rect.width;
            viewPos.y = viewPos.y * parent.rect.height;
            rectTransform.anchoredPosition = viewPos;
        }
    }
}

