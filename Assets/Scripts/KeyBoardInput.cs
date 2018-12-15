using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : BaseInput {

    public override void UpdateInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, new Vector2(1,0));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.RightMovement, new Vector2(1,0));
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.UpMovement, new Vector2(1,0));
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.DownMovement, new Vector2(1,0));
        }
    }
}
