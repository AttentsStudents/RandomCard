using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProperty : MonoBehaviour
{
    Image _img;
    public Image image
    {
        get
        {
            if (_img == null) _img = GetComponentInChildren<Image>();
            return _img;
        }
    }
}

