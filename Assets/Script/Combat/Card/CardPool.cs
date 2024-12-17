using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public static CardPool inst { get; set; }
    public Transform pool;
    List<GameObject> objects = new List<GameObject>();
    void Awake()
    {
        inst = this;
    }

    public void Push(GameObject obj) {
        objects.Add(obj);
        obj.transform.SetParent(pool);
    }

    public GameObject Pop() {
        if(objects.Count == 0) return null;
        int rIdx = Random.Range(0, objects.Count);
        GameObject obj = objects[rIdx];
        objects.Remove(obj);
        return obj;
    }
}
