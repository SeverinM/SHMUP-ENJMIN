using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : BaseInput {

    public override void UpdateInput()
    {
        if (BaseInput.IsFree(Actions.LeftMovement,this))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, new Vector2(1, 0));
                BaseInput.SetLockState(Actions.LeftMovement, this);
            }
            else
            {
                BaseInput.SetLockState(Actions.LeftMovement, null);
            }
        }

        if (BaseInput.IsFree(Actions.RightMovement, this))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                RaiseEvent(TypeAction.Pressed, Actions.RightMovement, new Vector2(1, 0));
                BaseInput.SetLockState(Actions.RightMovement, this);
            }
            else
            {
                BaseInput.SetLockState(Actions.RightMovement, null);
            }
        }

        if (BaseInput.IsFree(Actions.UpMovement, this))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                RaiseEvent(TypeAction.Pressed, Actions.UpMovement, new Vector2(1, 0));
                BaseInput.SetLockState(Actions.UpMovement, this);
            }
            else
            {
                BaseInput.SetLockState(Actions.UpMovement, null);
            }
        }

        if (BaseInput.IsFree(Actions.DownMovement, this))
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                RaiseEvent(TypeAction.Pressed, Actions.DownMovement, new Vector2(1, 0));
                BaseInput.SetLockState(Actions.DownMovement, this);
            }
            else
            {
                BaseInput.SetLockState(Actions.DownMovement, null);
            }
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

        //RaiseEvent(TypeAction.Mouse, Actions.Movement, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        
    }
}
