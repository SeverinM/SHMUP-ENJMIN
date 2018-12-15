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
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.RightMovement))
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


        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.Movement))
        {

            Vector3 objectPos = Camera.main.WorldToScreenPoint(character.transform.position);
            val.x = val.x - objectPos.x;
            val.y = val.y - objectPos.y;

            float angle = Mathf.Atan2(val.x, val.y) * Mathf.Rad2Deg;
            character.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

        }

        direction.Normalize();
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
