using UnityEngine;
using System.Collections;


/// <summary>
/// Etat "Idle" du joueur , il peut se deplacer librement et orienter son perso comme il le souhaite
/// C'est le seul etat etant exclusif au joueur
/// </summary>
public class PlayerMovement : State
{
    protected Vector2 direction;
    Player.MovementMode mode;

    protected float dashDistance;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
        mode = character.Context.ValuesOrDefault<Player.MovementMode>("Mode", Player.MovementMode.Normal);
        dashDistance = character.Context.ValuesOrDefault<float>("RangeDash", 10);
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Mode normal
        if (mode.Equals(Player.MovementMode.Normal))
        {
            //Un mouvement quelconque (manette / souris) est detecté
            if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement) && character.GetScale() * character.PersonalScale > 0)
            {
                direction.Set(val.x, val.y);
                direction *= character.GetScale();
                character.Move(direction);
            }

            //rotation stick droit
            if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.RotateAbsolute) && character.GetScale() * character.PersonalScale > 0)
            {
                character.transform.eulerAngles = new Vector3(0, val.x, 0);
            }
        }

        //Mode dash
        if (mode.Equals(Player.MovementMode.Dash))
        {
            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash) && character.GetScale() * character.PersonalScale > 0)
            {
                character.transform.position += character.transform.forward * dashDistance;
            }

            //rotation stick gauche
            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.AllMovement) && character.GetScale() * character.PersonalScale > 0)
            {
                val.Normalize();
                float value = Mathf.Acos(val.x) * Mathf.Rad2Deg;
                if (val.y > 0)
                {
                    value *= -1;
                }
                value += 90;

                //Rotation ET dash
                character.transform.eulerAngles = new Vector3(0, value, 0);
                character.transform.position += character.transform.forward * dashDistance;
            }
        }

        if(typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Shoot))
        {
            NextState();
        }

        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.Rotate) && character.GetScale() * character.PersonalScale > 0)
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
        ((Player)character).UpdateHook();
    }

    public override void NextState()
    {
        character.SetState(new PlayerShoot(character));
    }

    public override void StartState()
    {
        ((Player)character).ResetHook();
    }
}
