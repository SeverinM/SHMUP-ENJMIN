using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInput {

    public enum TypeAction
    {
        Down,
        Up,
        Pressed,
        Mouse
    }

    public enum Actions
    {
        LeftMovement,
        RightMovement,
        UpMovement,
        DownMovement,
        Shoot,
        Movement,
    }

    protected void RaiseEvent(TypeAction tyAct, Actions acts, Vector2 values)
    {
        OnInputExecuted.Invoke(tyAct, acts, values);
    }

    public abstract void UpdateInput();

    public delegate void InputEvent(TypeAction tyAct, Actions acts, Vector2 values);
    public event InputEvent OnInputExecuted;
}
