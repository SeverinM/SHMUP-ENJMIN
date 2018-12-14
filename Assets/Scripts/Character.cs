using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [SerializeField] protected float moveSpeed = 6.0f;
    [SerializeField] private State actualState;

    public void SetState(State state)
    {
        actualState.EndState();
        actualState = state;
        actualState.StartState();
    }
}
