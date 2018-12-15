using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : BaseInput {

    public override void UpdateInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, Vector2.zero);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.RightMovement, Vector2.zero);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.UpMovement, Vector2.zero);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.DownMovement, Vector2.zero);
        }

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
    }
}
