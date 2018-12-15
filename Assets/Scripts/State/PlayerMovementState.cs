using UnityEngine;
using System.Collections;

public class PlayerMovementState : State
{
    private Vector2 direction;

    public PlayerMovementState(Character character) : base(character)
    {
        direction = new Vector2();
    }

    public override void EndState()
    {
        
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if(typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.RightMovement))
        {
            direction.Set(1,0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.LeftMovement))
        {
            direction.Set(-1, 0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.UpMovement))
        {
            direction.Set(0, 1);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.DownMovement))
        {
            direction.Set(0, -1);
        }

        if(typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Shoot))
        {
            NextState();
        }
     
        direction.Normalize();
        character.Move(direction);

    }

    public override void NextState()
    {
        character.SetState(new PlayerShootState(character));
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
