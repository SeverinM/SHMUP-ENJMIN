using UnityEngine;
using System.Collections;

public delegate void InterpretInput();

public class Player : Character
{
 
    private CharacterController controller;
    public event InterpretInput InterpretInput;

    void Start()
    {
        controller = transform.GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }



}
