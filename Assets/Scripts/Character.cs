using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public delegate void collDelegate(Collider coll);
    public event collDelegate OnTriggerEnterChar;

    [SerializeField]
    protected float moveSpeed = 6.0f;

    [SerializeField]
    protected State actualState;
    public State ActualState
    {
        get
        {
            return actualState;
        }
    }

    public void SetState(State state)
    {
        if (actualState != null)
        {
            actualState.EndState();
        }
        actualState = state;

        if (actualState != null)
        {
            actualState.StartState();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (OnTriggerEnterChar != null)
            OnTriggerEnterChar(other);
    }

    protected void Update()
    {
        if (actualState != null)
        {
            actualState.UpdateState();
        }
    }

    public virtual void Move(Vector2 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, 0, movement.y * Time.deltaTime * moveSpeed), Space.World);
    }

    public virtual void Move(Vector3 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, movement.y * Time.deltaTime * moveSpeed, movement.z * Time.deltaTime * moveSpeed), Space.World);
    }

}
