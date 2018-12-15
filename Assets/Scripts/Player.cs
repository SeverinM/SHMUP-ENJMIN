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

}
