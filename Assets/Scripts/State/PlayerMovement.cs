using UnityEngine;
using System.Collections;

public class PlayerMovement : State
{
    protected Vector2 direction;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.RightMovement))
        {
            direction.Set(val.x, 0);
            character.Move(direction);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.LeftMovement))
        {
            direction.Set(-val.x, 0);
            character.Move(direction);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.UpMovement))
        {
            direction.Set(0, val.x);
            character.Move(direction);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.DownMovement))
        {
            direction.Set(0, -val.x);
            character.Move(direction);
        } 


        if(typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Shoot))
        {
            NextState();
        }

        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.Rotate))
        {

            Vector3 objectPos = Camera.main.WorldToScreenPoint(character.transform.position);
            Vector2 mousePos = new Vector2();
            mousePos.x = val.x - objectPos.x;
            mousePos.y = val.y - objectPos.y;

            float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
            character.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

        }

        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.RotateAbsolute))
        {
            character.transform.eulerAngles = new Vector3(0, val.x, 0);
        }
    }

    public override void NextState()
    {
        character.SetState(new PlayerShoot(character));
    }
}
