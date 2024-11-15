using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CheonJiWoon
{
    public class ImageProperty : MonoBehaviour
    {
        Image _img;
        public Image image
        {
            get
            {
                if (_img == null) _img = GetComponent<Image>();
                return _img;
            }
        }
    }
}

