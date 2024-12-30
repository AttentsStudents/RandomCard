using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TMP_Text text;
    void Start()
    {
        StartCoroutine(Effect());
    }

    IEnumerator Effect()
    {
        float time = 0.6f;
        while(time > 0.0f)
        {
            float delta = Time.deltaTime;
            transform.Translate(Vector3.up * delta * 80.0f);
            time -= delta;
            yield return null;
        }
        Destroy(gameObject);
    }
}
