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

    private float dashMultiplicator = 1.2f;

    private float dashCooldownTime;

    private float dashing;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
        mode = character.Context.ValuesOrDefault<Player.MovementMode>("Mode", Player.MovementMode.Normal);
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Mode normal
        if (mode.Equals(Player.MovementMode.Normal))
        {
            //Un mouvement quelconque (manette / souris) est detecté
            if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement) && character.GetScale() * character.PersonalScale > 0)
            {
                float size = character.GetComponent<BoxCollider>().size.magnitude;
                direction.Set(val.x, val.y);
                if (Utils.IsInCamera(character.transform.position + (new Vector3(direction.x,0,direction.y) * size * 0.5f), Mathf.Abs(character.transform.position.y - Camera.main.transform.position.y)))
                {
                    character.Move(direction);
                }
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
                    dashing = ((Player)character).DistanceDash/100;
                    AkSoundEngine.PostEvent("S_Dash", character.gameObject);
                }
            }
        }


        //Mode dash
        if (mode.Equals(Player.MovementMode.Dash))
        {
            // start dash
            if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash) && character.GetScale() * character.PersonalScale > 0)
            {
                character.transform.position += character.transform.forward * ((Player)character).DistanceDash;
                AkSoundEngine.PostEvent("S_Dash", character.gameObject);
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
                character.transform.position += character.transform.forward * ((Player)character).DistanceDash;
            }
        }

        if(typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Shoot))
        {
            NextState();
        }

        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.Rotate) && character.GetScale() * character.PersonalScale > 0 && !(dashing > 0f))
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
        Vector3 forward = character.transform.TransformDirection(Vector3.forward);

        if (dashing > 0)
        {
            dashing -= Time.deltaTime;

            character.Move(forward * ((Player)character).dashSpeed);
        } else
        {
            dashCooldownTime += Time.deltaTime;
            if(dashCooldownTime > ((Player)character).dashCooldown && ((Player)character).Dash < ((Player)character).maxDashes)
            {
                ((Player)character).Dash++;
                dashCooldownTime = 0;
            }
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
