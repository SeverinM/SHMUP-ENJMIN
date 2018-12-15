using UnityEngine;
using System.Collections;

public class PlayerMovement : State
{
    private Vector2 direction;

    public PlayerMovement(Character character) : base(character)
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
            direction.Set(val.x, 0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.LeftMovement))
        {
            direction.Set(-val.x, 0);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.UpMovement))
        {
            direction.Set(0, val.x);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.DownMovement))
        {
            direction.Set(0, -val.x);
        }


        if(typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Shoot))
        {
            NextState();
        }


        character.Move(direction);

    }

    public override void NextState()
    {
        character.SetState(new PlayerShoot(character));
    }

    public override void StartState()
    {
        
    }

    public override void UpdateState()
    {
    }
}
