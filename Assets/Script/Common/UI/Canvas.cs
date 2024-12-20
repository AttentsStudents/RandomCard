using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas : MonoBehaviour
{
    public static Canvas main;
    public Transform hpBars;
    void Awake() => InitCanvas();

    protected void InitCanvas() => main = this;
}
