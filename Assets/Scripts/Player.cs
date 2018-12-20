﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : Character
{
 
    public enum MovementMode
    {
        Dash,
        Normal
    }

    [SerializeField]
    MovementMode mode;
    public MovementMode Mode
    {
        get
        {
            return mode;
        }
    }

    [SerializeField]
    private Transform hook;
    public Transform Hook
    {
        get
        {
            return hook;
        }
    }

    void Start()
    {
        actualState = new PlayerMovement(this);
    }

    new void Update()
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


    void OnCollisionEnter(Collision collision)
    {
        // On bullet
        if (collision.gameObject.name.Contains("Bullet"))
        {
            Impact(collision.relativeVelocity * hitForce);
            Destroy(collision.gameObject);
        }
    }

    public void Impact(Vector3 force)
    {
        Vector3 dir = force.normalized;
        dir.y = 0.5f;
        impact += dir.normalized * force.magnitude / mass;
    }

}
