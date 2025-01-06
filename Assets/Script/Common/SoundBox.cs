using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : SingleTon<SoundBox>
{
    public AudioSource aSrc;

    void Awake()
    {
        Init();
        aSrc.time = 0.2f;
    }

    public static void PlayOneShot(AudioClip clip) => inst.aSrc.PlayOneShot(clip);
}
