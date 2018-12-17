using UnityEngine;
using System.Collections;

public class PlayerMovement : State
{
    protected Vector2 direction;
    Player.MovementMode mode;
    Transform hook;
    Vector3 origin;

    protected float dashDistance = 2f;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
        mode = ((Player)character).Mode;
        hook = character.transform.GetChild(0);
        origin = hook.localPosition;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Mode normal
        if (mode.Equals(Player.MovementMode.Normal))
        {
            if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement))
            {
                direction.Set(val.x, val.y);
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

            //rotation stick gauche
            if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement))
            {
                float value = Mathf.Acos(val.x) * Mathf.Rad2Deg;
                if (val.y > 0)
                {
                    value *= -1;
                }
                value += 90;
                character.transform.eulerAngles = new Vector3(0, value, 0);
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
            character.transform.localEulerAngles = new Vector3(0, angle, 0);
        }
    }

    public override void UpdateState()
    {
        hook.forward = character.transform.forward;
        hook.localPosition = origin;
    }

    public override void NextState()
    {
        character.SetState(new PlayerShoot(character));
    }
}
