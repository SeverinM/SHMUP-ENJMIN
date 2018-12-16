using UnityEngine;
using System.Collections;

public class PlayerMovement : State
{
    protected Vector2 direction;
    protected Vector3 relativePos;
    Player.MovementMode mode;

    protected float dashDistance = 2f;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
        relativePos = character.transform.GetChild(0).position - character.transform.position;
        mode = ((Player)character).Mode;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Mode normal
        if (mode.Equals(Player.MovementMode.Normal))
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

            //rotation stick droit
            if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.RotateAbsolute))
            {
                character.transform.eulerAngles = new Vector3(0, val.x, 0);
            }
        }

        //Mode dash
        if (mode.Equals(Player.MovementMode.Dash))
        {
            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash))
            {
                character.transform.position += character.transform.forward * dashDistance;
            }

            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash))
            {
                character.transform.position += character.transform.forward * dashDistance;
            }

            //rotation stick gauche
            if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement))
            {
                Debug.Log(val.x);
                character.transform.eulerAngles = new Vector3(0, val.x, 0);
            }
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
    }

    public override void UpdateState()
    {
        character.transform.GetChild(0).forward = character.transform.forward;
    }

    public override void NextState()
    {
        character.SetState(new PlayerShoot(character));
    }
}
