using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimProperty : MonoBehaviour
{
    Animator _anim;
    public Animator anim
    {
        get
        {
            if (_anim == null) _anim = GetComponentInChildren<Animator>();
            return _anim;
        }
        set
        {
            _anim = value;
        }
    }
}
