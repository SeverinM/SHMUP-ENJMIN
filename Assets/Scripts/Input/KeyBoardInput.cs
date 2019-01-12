using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input gérés par le clavier
/// </summary>
public class KeyBoardInput : BaseInput {
    bool wasDown = false;
    Vector2 movements = Vector2.zero;

    public override void UpdateInput()
    {
        movements = Vector2.zero;
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && BaseInput.IsFree(Actions.AllMovement,this))
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movements += Vector2.down;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                movements += Vector2.up;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movements += Vector2.left;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                movements += Vector2.right;
            }

            RaiseEvent(TypeAction.Pressed, Actions.AllMovement, movements);

            if (!wasDown)
            {
                wasDown = true;
                RaiseEvent(TypeAction.Down, Actions.AllMovement, movements);
            }
        }

        else
        {
            if (wasDown)
            {
                RaiseEvent(TypeAction.Up, Actions.AllMovement, movements);
                wasDown = false;
                BaseInput.SetLockState(Actions.AllMovement, null);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("jo");
            RaiseEvent(TypeAction.Down, Actions.Pause, Vector2.zero);
        }

    }
}
