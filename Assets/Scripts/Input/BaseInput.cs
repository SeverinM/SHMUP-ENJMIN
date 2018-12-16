using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInput {

    public static Dictionary<Actions,BaseInput> lockController = new Dictionary<Actions,BaseInput>();

    public static BaseInput GetLockState(Actions acts)
    {
        if (!lockController.ContainsKey(acts))
        {
            lockController[acts] = null;
        }
        return lockController[acts];
    }

    public static void SetLockState(Actions acts, BaseInput value)
    {
        lockController[acts] = value;
    }

    public static bool IsFree(Actions acts, BaseInput asker)
    {
        //Si le demandeur a deja le verrou ou qu'il n'y a pas de verrou
        if (GetLockState(acts) == null || GetLockState(acts) == asker)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public enum TypeAction
    {
        Down,
        Up,
        Pressed,
        Mouse
    }

    public enum Actions
    {
        AllMovement,
        Shoot,
        Rotate,
        RotateAbsolute,
        Dash
    }

    protected void RaiseEvent(TypeAction tyAct, Actions acts, Vector2 values)
    {
        OnInputExecuted.Invoke(tyAct, acts, values);
    }

    public abstract void UpdateInput();

    public delegate void InputEvent(TypeAction tyAct, Actions acts, Vector2 values);
    public event InputEvent OnInputExecuted;
}
