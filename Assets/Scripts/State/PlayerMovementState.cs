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
        throw new System.NotImplementedException();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if(typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.RightMovement))
        {
            Debug.Log(move);
            move.Set(1,0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.LeftMovement))
        {
            Debug.Log(move);
            move.Set(-1, 0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.UpMovement))
        {
            Debug.Log(move);
            move.Set(0, 1);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.DownMovement))
        {
            Debug.Log(move);
            move.Set(0, -1);
        }
     
        //move.Normalize();
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
        throw new System.NotImplementedException();
    }
}
