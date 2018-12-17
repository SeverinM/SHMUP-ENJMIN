using UnityEngine;
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

    private CharacterController controller;
    public CharacterController Controller
    {
        get
        {
            return controller;
        }
    }

    void Start()
    {
      

        actualState = new PlayerMovement(this);
    }

    public void InterpretInput(BaseInput.TypeAction typAct, BaseInput.Actions baseInput , Vector2 value)
    {
        if (actualState != null)
        {
            actualState.InterpretInput(typAct, baseInput, value);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // On bullet

        Impact(collision.relativeVelocity * hitForce);
        
    }

}
