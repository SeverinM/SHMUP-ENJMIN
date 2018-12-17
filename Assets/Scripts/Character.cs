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
    protected float mass = 3.0f;                
    protected float hitForce = 2.5f;            
    protected Vector3 impact = Vector3.zero; 

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

        if (impact.magnitude > 0.2)
        { // if momentum > 0.2 
            Move(impact * Time.deltaTime); // move character
        }
        // impact vanishes to zero over time
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public virtual void Move(Vector2 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, 0, movement.y * Time.deltaTime * moveSpeed), Space.World);
    }

    public virtual void Move(Vector3 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, movement.y * Time.deltaTime * moveSpeed, movement.z * Time.deltaTime * moveSpeed), Space.World);
    }

    public virtual void Impact(Vector3 force)
    {
        Vector3 dir = force.normalized;
        dir.y = 0.5f;
        impact += dir.normalized * force.magnitude / mass;
    }
}
