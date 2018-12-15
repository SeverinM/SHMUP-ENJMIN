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
    
    public void Test(BaseInput.TypeAction typAct, BaseInput.Actions baseInput , Vector2 value)
    {
        if (typAct == BaseInput.TypeAction.Pressed && baseInput == BaseInput.Actions.LeftMovement)
        {
            Debug.Log("gauche");
        }

        if (typAct == BaseInput.TypeAction.Pressed && baseInput == BaseInput.Actions.RightMovement)
        {
            Debug.Log("droite");
        }
    }

}
