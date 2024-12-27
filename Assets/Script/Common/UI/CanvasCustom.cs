using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCustom : MonoBehaviour
{
    public static CanvasCustom main;
    public Transform priortyLoad;
    public Transform lastLoad;
    void Awake() => InitCanvas();

    protected void InitCanvas() => main = this;
}
