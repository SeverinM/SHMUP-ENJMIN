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
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.D)) && BaseInput.IsFree(Actions.AllMovement,this))
        {
            if (Input.GetKey(KeyCode.S))
            {
                movements += Vector2.down;
            }

            if (Input.GetKey(KeyCode.Z))
            {
                movements += Vector2.up;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                movements += Vector2.left;
            }

            if (Input.GetKey(KeyCode.D))
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
            RaiseEvent(TypeAction.Down, Actions.Pause, Vector2.zero);
        }

    }
}
