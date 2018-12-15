using UnityEngine;
using System.Collections;

public class MouseInput : BaseInput
{
    public override void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaiseEvent(TypeAction.Down, Actions.Shoot, Vector2.zero);
        }

        if (Input.GetMouseButton(0))
        {
            RaiseEvent(TypeAction.Pressed, Actions.Shoot, Vector2.zero);
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaiseEvent(TypeAction.Up, Actions.Shoot, Vector2.zero);
        }

        RaiseEvent(TypeAction.Mouse, Actions.Movement, Input.mousePosition);
    }
}
