using UnityEngine;
using System.Collections;

public delegate void InterpretInput();

public class Player : Character
{
 
    private CharacterController controller;
    public CharacterController Controller => controller;

    void Start()
    {
        if (!GetComponent<CharacterController>())
        {
            gameObject.AddComponent<CharacterController>();
        }
        controller = transform.GetComponent<CharacterController>();

        actualState = new PlayerMovementState(this);
    }

    void Update()
    {
        
    }
    
    public void InterpretInput(BaseInput.TypeAction typAct, BaseInput.Actions baseInput , Vector2 value)
    {
        if (actualState != null)
        {
            actualState.InterpretInput(typAct, baseInput, value);
        }
    }

    public override void Move(Vector2 movement)
    {
        transform.Translate(movement.x * Time.deltaTime * moveSpeed, 0, movement.y * Time.deltaTime * moveSpeed);
    }
}
