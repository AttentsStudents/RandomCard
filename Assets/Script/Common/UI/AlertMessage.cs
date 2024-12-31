using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertMessage : MonoBehaviour
{
    public static void Alert(string msg)
    {
        AlertMessage alert = Instantiate(ObjectManager.inst.alertMessage, CanvasCustom.main.transform).GetComponent<AlertMessage>();
        alert.tmpText.text = msg;
    }

    public RectTransform rect;
    public TMP_Text tmpText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        Vector2 rectSize = rect.sizeDelta;
        rectSize.x = tmpText.preferredWidth + 120;
        rect.sizeDelta = rectSize;
        yield return WaitForSecondsCustom.Get(1.5f);
        Destroy(gameObject);
    }
}
