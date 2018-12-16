using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementDuringHook : PlayerMovement {

    Vector3 originRelative;
    Transform hook;

    public PlayerMovementDuringHook(Character chara,Transform hook, Vector3 origin) : base(chara)
    {
        originRelative = origin;
        this.hook = hook;
    }

    public override void NextState()
    {
        character.SetState(new PlayerWinch(character, hook, originRelative));
    }

    public override void UpdateState()
    {
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        Vector3 copy = hook.transform.position;
        val /= 10;
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement))
        {
            direction.Set(val.x, val.y);
            character.Move(direction);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            NextState();
        }

        hook.GetComponent<LineRenderer>().SetPosition(1, character.transform.position);
        hook.transform.position = copy;
    }
}
