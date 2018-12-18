using UnityEngine;
using System.Collections;

/// <summary>
/// Input gérés par la souris
/// </summary>
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

        if (Input.GetMouseButtonDown(1))
        {
            RaiseEvent(TypeAction.Down, Actions.Dash, Vector2.zero);
        }

        if (Input.GetMouseButton(1))
        {
            RaiseEvent(TypeAction.Pressed, Actions.Dash, Vector2.zero);
        }

        if (Input.GetMouseButtonUp(1))
        {
            RaiseEvent(TypeAction.Up, Actions.Dash, Vector2.zero);
        }

        //On envoi un event que si la souris a bougé
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            RaiseEvent(TypeAction.Mouse, Actions.Rotate, Input.mousePosition);
        }       
    }
}
