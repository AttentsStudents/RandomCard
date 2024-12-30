using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaitForSecondsCustom
{
    static Dictionary<float, WaitForSeconds> dictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds Get(float seconds)
    {
        if (dictionary.ContainsKey(seconds)) return dictionary[seconds];
        return dictionary[seconds] = new WaitForSeconds(seconds);
    }
}