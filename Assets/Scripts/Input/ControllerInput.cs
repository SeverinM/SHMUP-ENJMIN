using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : BaseInput {


    public override void UpdateInput()
    {
        float xValue = Input.GetAxis("HorizontalController");
        float yValue = Input.GetAxis("VerticalController");

        //La voie est libre, aucun autre controlleur n'est manipulé
        if (xValue > 0 && BaseInput.IsFree(Actions.RightMovement, this) && BaseInput.IsFree(Actions.LeftMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.RightMovement, new Vector2(Mathf.Abs(xValue), 0));
            BaseInput.SetLockState(Actions.RightMovement, this);
            BaseInput.SetLockState(Actions.LeftMovement, this);
        }

        if (xValue < 0 && BaseInput.IsFree(Actions.RightMovement, this) && BaseInput.IsFree(Actions.LeftMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, new Vector2(Mathf.Abs(xValue), 0));
            BaseInput.SetLockState(Actions.RightMovement, this);
            BaseInput.SetLockState(Actions.LeftMovement, this);
        }

        if (yValue < 0 && BaseInput.IsFree(Actions.UpMovement, this) && BaseInput.IsFree(Actions.DownMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.DownMovement, new Vector2(Mathf.Abs(yValue), 0));
            BaseInput.SetLockState(Actions.UpMovement, this);
            BaseInput.SetLockState(Actions.DownMovement, this);
        }

        if (yValue > 0 && BaseInput.IsFree(Actions.UpMovement, this) && BaseInput.IsFree(Actions.DownMovement, this))
        {
            RaiseEvent(TypeAction.Pressed, Actions.UpMovement, new Vector2(Mathf.Abs(yValue), 0));
            BaseInput.SetLockState(Actions.UpMovement, this);
            BaseInput.SetLockState(Actions.DownMovement, this);
        }

        if (xValue == 0 && BaseInput.IsFree(Actions.RightMovement, this) && BaseInput.IsFree(Actions.LeftMovement, this))
        {
            BaseInput.SetLockState(Actions.RightMovement, null);
            BaseInput.SetLockState(Actions.LeftMovement, null);
        }

        if (yValue == 0 && BaseInput.IsFree(Actions.UpMovement, this) && BaseInput.IsFree(Actions.DownMovement, this))
        {
            BaseInput.SetLockState(Actions.RightMovement, null);
            BaseInput.SetLockState(Actions.LeftMovement, null);
        }
    }
}
