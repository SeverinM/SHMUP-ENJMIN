using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : BaseInput {


    public override void UpdateInput()
    {
        float xValue = Input.GetAxis("Horizontal");
        float yValue = Input.GetAxis("Vertical");

        if (xValue > 0)
        {
            RaiseEvent(TypeAction.Pressed, Actions.RightMovement, new Vector2(Mathf.Abs(xValue), 0));
        }

        if (xValue < 0)
        {
            RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, new Vector2(Mathf.Abs(xValue), 0));
        }

        if (yValue < 0)
        {
            RaiseEvent(TypeAction.Pressed, Actions.DownMovement, new Vector2(Mathf.Abs(yValue), 0));
        }

        if (yValue > 0)
        {
            RaiseEvent(TypeAction.Pressed, Actions.UpMovement, new Vector2(Mathf.Abs(yValue), 0));
        }
    }
}
