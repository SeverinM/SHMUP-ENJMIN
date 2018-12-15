﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : BaseInput {

    public override void UpdateInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.LeftMovement, Vector2.zero);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            RaiseEvent(TypeAction.Pressed, Actions.RightMovement, Vector2.zero);
        }
    }
}
