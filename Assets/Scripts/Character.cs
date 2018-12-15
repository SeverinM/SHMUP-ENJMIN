using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    [SerializeField]
    protected float moveSpeed = 6.0f;

    [SerializeField]
    protected State actualState;

    public void SetState(State state)
    {
        actualState.EndState();
        actualState = state;
        actualState.StartState();
    }

    public abstract void Move(Vector2 movement);
}
