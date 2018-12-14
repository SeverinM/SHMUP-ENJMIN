using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInput{

    public enum TypeAction
    {
        Down,
        Up,
        Pressed
    }

    public enum Actions
    {
        LeftMovement,
        RightMovement,
        UpMovement,
        DownMovement,
        Shoot
    }

    public abstract void UpdateInput();

    public delegate void InputEvent(TypeAction tyAct, Actions acts, Vector2 values);
    public event InputEvent OnInputExecuted;
}
