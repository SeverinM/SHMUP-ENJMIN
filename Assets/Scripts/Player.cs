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
        if (!GetComponent<CharacterController>())
        {
            gameObject.AddComponent<CharacterController>();
        }
        controller = transform.GetComponent<CharacterController>();

        actualState = new PlayerMovement(this);
    }
}
