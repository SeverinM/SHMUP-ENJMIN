using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [SerializeField] protected float moveSpeed = 6.0f;
    [SerializeField] private State actualState;

    protected BaseInput input;

    public void SetState(State state)
    {
        actualState.EndState();
        actualState = state;
        actualState.StartState();
    }


    public void AddInput(BaseInput input)
    {
        this.input = input;
    }
}
