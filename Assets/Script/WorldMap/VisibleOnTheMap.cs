using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public abstract class VisibleOnTheMap : MonoBehaviour
    {
        public Sprite sprite;
        public bool dynamic;
        [Range(0.0f, 1.0f)]
        public float colorAlpha = 1.0f;
        protected void ViewOnTheMap()
        {
            string type = dynamic ? "DynamicMapIcon" : "MapIcon";
            MapIcon mapIcon = Instantiate(Resources.Load<GameObject>($"{SceneData.prefabPath}/{type}"), Map.instance.content).GetComponent<MapIcon>();
            mapIcon.target = transform;
            mapIcon.image.sprite = sprite;
            Color color = mapIcon.image.color;
            color.a = colorAlpha;
            mapIcon.image.color = color;
        }
    }
}

