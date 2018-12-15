using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Character
{
 
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
        if (!GetComponent<CharacterController>())
        {
            gameObject.AddComponent<CharacterController>();
        }
        controller = transform.GetComponent<CharacterController>();

        actualState = new PlayerMovement(this);
    }
    
    public void InterpretInput(BaseInput.TypeAction typAct, BaseInput.Actions baseInput , Vector2 value)
    {
        if (actualState != null)
        {
            actualState.InterpretInput(typAct, baseInput, value);
        }
    }
}
