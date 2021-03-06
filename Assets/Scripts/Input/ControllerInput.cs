﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input gérés par la manette
/// </summary>
public class ControllerInput : BaseInput {
    //Le joystick gauche n'est pas touché ?
    bool neutral = true;

    //Valeurs x / y du joystick gauche
    float xValue;
    float yValue;

    //Valeurs x / y du joystick droit
    float sinValue;
    float cosValue;

    //Valeur d'appui des deux gachettes , uniquement gauche enfoncé = -1 , uniquement droite enfoncé = 1 , aucune enfoncé ou les deux enfoncés = 0
    float triggerLeft;
    float triggerRight;

    float previousLeft = 0;
    float previousRight = 0;

    public override void UpdateInput()
    {
        xValue = Input.GetAxis("HorizontalController");
        yValue = Input.GetAxis("VerticalController");

        sinValue = Input.GetAxis("VerticalControllerRight");
        cosValue = Input.GetAxis("HorizontalControllerRight");

        triggerLeft = Input.GetAxis("TriggerLeft");
        triggerRight = Input.GetAxis("TriggerRight");

        xValue = Mathf.Clamp(xValue, -0.999f, 0.9999f);
        yValue = Mathf.Clamp(yValue, -0.999F, 0.9999f);

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
                RaiseEvent(TypeAction.Up, Actions.AllMovement, new Vector2(xValue, yValue));
                BaseInput.SetLockState(Actions.AllMovement, null);
            }
        }

        if (triggerRight > 0)
        {
            if (previousRight == 0)
                RaiseEvent(TypeAction.Down, Actions.Shoot, Vector2.zero);
            RaiseEvent(TypeAction.Pressed, Actions.Shoot, Vector2.zero);
        }
        if (triggerRight == 0 && previousRight > 0)
            RaiseEvent(TypeAction.Up, Actions.Shoot, Vector2.zero);

        if (triggerLeft > 0)
        {
            if (previousLeft == 0)
                RaiseEvent(TypeAction.Down, Actions.Dash, Vector2.zero);
            RaiseEvent(TypeAction.Pressed, Actions.Dash, Vector2.zero);
        }
        if (triggerLeft == 0 && previousLeft > 0)
            RaiseEvent(TypeAction.Up, Actions.Dash, Vector2.zero);

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

        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            RaiseEvent(TypeAction.Down, Actions.Pause, Vector2.zero);
        }

        previousLeft = triggerLeft;
        previousRight = triggerRight;
    }
}
