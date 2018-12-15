using UnityEngine;
using System.Collections;

public class PlayerMovementState : State
{
    Vector2 move;

    public PlayerMovementState(Character character) : base(character)
    {
        move = new Vector2();
    }

    public override void EndState()
    {
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if(typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.RightMovement))
        {
            move.Set(val.x,0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.LeftMovement))
        {
            move.Set(-val.x, 0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.UpMovement))
        {
            move.Set(0, val.x);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.DownMovement))
        {
            move.Set(0, -val.x);
        }
     
        character.Move(move);

    }

    public override void NextState()
    {
        throw new System.NotImplementedException();
    }

    public override void StartState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
    }
}
