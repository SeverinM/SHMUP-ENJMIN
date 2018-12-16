using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : BaseInput {

    bool wasDown = false;
    bool neutral = true;

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

        if ((xValue != 0 || yValue != 0) && BaseInput.IsFree(Actions.AllMovement,this))
        {
            if (neutral)
            {
                BaseInput.SetLockState(Actions.AllMovement, this);
                neutral = false;
                RaiseEvent(TypeAction.Down, Actions.AllMovement, new Vector2(xValue, yValue));
            }

            RaiseEvent(TypeAction.Pressed, Actions.AllMovement, new Vector2(xValue, yValue));
        }

        else
        {
            if (!neutral && BaseInput.IsFree(Actions.AllMovement, this))
            {
                neutral = true;
                RaiseEvent(TypeAction.Up, Actions.AllMovement, Vector2.zero);
                BaseInput.SetLockState(Actions.AllMovement, null);
            }
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

        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            RaiseEvent(TypeAction.Down, Actions.Dash, Vector2.zero);
        }

        if (Input.GetKey(KeyCode.Joystick1Button0))
        {
            RaiseEvent(TypeAction.Pressed, Actions.Dash, Vector2.zero);
        }

        if (Input.GetKeyUp(KeyCode.Joystick1Button0))
        {
            RaiseEvent(TypeAction.Up, Actions.Dash, Vector2.zero);
        }
    }
}
