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

    private float dashTime = 0.2f;
    private float dashMultiplicator = 1.2f;

    private float dashSpeed;

    private float dashCooldownTime;

    private float dashing;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
        mode = character.Context.ValuesOrDefault<Player.MovementMode>("Mode", Player.MovementMode.Normal);
        dashDistance = ((Player)character).DistanceDash;
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

        if (mode.Equals(Player.MovementMode.NormalDash))
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
                dashing = 0; // Stop dashing
            }

            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash) && character.GetScale() * character.PersonalScale > 0)
            {
                if (((Player)character).Dash > 0) // StartDashing
                {
                    ((Player)character).Dash--;
                    dashing = dashTime; 
                }
            }
        }


        //Mode dash
        if (mode.Equals(Player.MovementMode.Dash))
        {
            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash) && character.GetScale() * character.PersonalScale > 0)
            {
                Debug.Log(dashDistance);
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

            dashing -= Time.deltaTime; // Reduce dashing

            float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
            character.transform.localEulerAngles = new Vector3(0, angle, 0);
        }
    }

    public override void UpdateState()
    {
        ((Player)character).UpdateHook();
        Vector3 forward = character.transform.TransformDirection(Vector3.forward);

        if (dashing > 0)
        {
            dashing -= Time.deltaTime;

            dashSpeed += dashMultiplicator;

            character.Move(forward * dashSpeed);
        } else
        {
            dashCooldownTime += Time.deltaTime;
            if(dashCooldownTime > ((Player)character).dashCooldown && ((Player)character).Dash < ((Player)character).maxDashes)
            {
                ((Player)character).Dash++;
                dashCooldownTime = 0;
            }
            dashSpeed = 0f;
        }
    }

    public override void NextState()
    {
        character.SetState(new PlayerShoot(character));
    }

    public override void StartState()
    {
        ((Player)character).ResetHook();
    }

    public override string GetName()
    {
        return "PlayerMovement";
    }
}
