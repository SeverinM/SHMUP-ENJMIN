using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : BaseInput {

    public override void UpdateInput()
    {
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && BaseInput.IsFree(Actions.RightMovement, this) && BaseInput.IsFree(Actions.LeftMovement, this))
        {
            BaseInput.SetLockState(Actions.RightMovement,this);
            BaseInput.SetLockState(Actions.LeftMovement,this);
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) && BaseInput.IsFree(Actions.UpMovement, this) && BaseInput.IsFree(Actions.DownMovement, this))
        {
            BaseInput.SetLockState(Actions.UpMovement, this);
            BaseInput.SetLockState(Actions.DownMovement, this);
        }

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && BaseInput.IsFree(Actions.DownMovement, this) && BaseInput.IsFree(Actions.UpMovement, this))
        {
            BaseInput.SetLockState(Actions.UpMovement, null);
            BaseInput.SetLockState(Actions.DownMovement, null);
        }

        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && BaseInput.IsFree(Actions.LeftMovement, this) && BaseInput.IsFree(Actions.RightMovement, this))
        {
            BaseInput.SetLockState(Actions.LeftMovement, null);
            BaseInput.SetLockState(Actions.RightMovement, null);
        }

        if (Input.GetKey(KeyCode.LeftArrow) && BaseInput.IsFree(Actions.RightMovement, this) && BaseInput.IsFree(Actions.LeftMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, new Vector2(1,0));
        }

        if (Input.GetKey(KeyCode.RightArrow) && BaseInput.IsFree(Actions.RightMovement, this) && BaseInput.IsFree(Actions.LeftMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.RightMovement, new Vector2(1,0));
        }

        if (Input.GetKey(KeyCode.UpArrow) && BaseInput.IsFree(Actions.UpMovement, this) && BaseInput.IsFree(Actions.DownMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.UpMovement, new Vector2(1,0));
        }

        if (Input.GetKey(KeyCode.DownArrow) && BaseInput.IsFree(Actions.UpMovement, this) && BaseInput.IsFree(Actions.DownMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.DownMovement, new Vector2(1,0));
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

        RaiseEvent(TypeAction.Mouse, Actions.Movement, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        
    }
}
