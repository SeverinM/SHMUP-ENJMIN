using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : BaseInput {

    bool wasDown = false;
    float xValue;
    float yValue;

    float sinValue;
    float cosValue;

    float trigger;

    public override void UpdateInput()
    {
        xValue = Input.GetAxis("HorizontalController");
        yValue = Input.GetAxis("VerticalController");

        sinValue = Input.GetAxis("VerticalControllerRight");
        cosValue = Input.GetAxis("HorizontalControllerRight");

        trigger = Input.GetAxis("Triggers");

        #region verouillage

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

        if (trigger != 0 && !wasDown)
        {
            RaiseEvent(TypeAction.Down, Actions.Shoot, Vector2.zero);
            wasDown = true;
        }

        if (trigger == 0 && wasDown)
        {
            RaiseEvent(TypeAction.Up, Actions.Shoot, Vector2.zero);
            wasDown = false;
        }

        if (sinValue * cosValue != 0)
        {
            float value = Mathf.Acos(cosValue) * Mathf.Rad2Deg;
            if (sinValue < 0)
            {
                value *= -1;
            }
            value += 90;
            RaiseEvent(TypeAction.Mouse, Actions.RotateAbsolute, new Vector2(value,0));
        }

        #endregion deverouillage

        #region deverouillage

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

        #endregion 
    }
}
