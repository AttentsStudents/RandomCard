using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimParameterData
{
    public int IsMove;
    public int OnAttack;
    public int IsAttack;
    public int OnDamage;
    public int OnDead;
    public int MyTurn;
    public int OnSkill;

    public void Initialize()
    {
        IsMove = Animator.StringToHash("IsMoving");
        OnAttack = Animator.StringToHash("OnAttack");
        IsAttack = Animator.StringToHash("IsAttack");
        OnDamage = Animator.StringToHash("OnDamage");
        OnDead = Animator.StringToHash("OnDead");
        MyTurn = Animator.StringToHash("MyTurn");
        OnSkill = Animator.StringToHash("OnSkill");
    }
}

public class AnimProperty : MonoBehaviour
{
    Animator _anim = null;
    protected internal AnimParameterData animData = new AnimParameterData();
    protected Animator myAnim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponent<Animator>();
                if (_anim == null)
                {
                    _anim = GetComponentInChildren<Animator>();
                }
                animData.Initialize();
            }
            return _anim;
        }
    }
}
