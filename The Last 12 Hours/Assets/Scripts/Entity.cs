using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Entity : Map
{
    [SerializeField]
    protected int _damage; //flat damage for this class

    [SerializeField]
    protected int _vision; //is this pixels? 

    [SerializeField]
    protected float Speed;

    [SerializeField]
    public bool canMove;

    protected abstract void Attack();
    protected abstract void Walk();

}
