using UnityEngine;
using System.Collections;


/// <summary>
/// Etat "Idle" du joueur , il peut se deplacer librement et orienter son perso comme il le souhaite
/// C'est le seul etat etant exclusif au joueur
/// </summary>
public class PlayerMovement : State
{
    protected Vector2 direction;
    Vector3 dashDirection = Vector3.zero;
    private float dashing;
    Vector3 lastMovement = Vector3.zero;
    bool didRotation = false;

    public PlayerMovement(Character character) : base(character)
    {
        direction = new Vector2();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        didRotation = false;

        //Un mouvement quelconque (manette / souris) est detecté
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement) && character.GetScale() * character.PersonalScale > 0 && dashing == 0)
        {
            float size = character.GetComponent<BoxCollider>().size.magnitude;
            float dist = Mathf.Abs(character.transform.position.y - Camera.main.transform.position.y);
            direction.Set(val.x, val.y);
            direction *= character.GetScale();
            dashDirection = new Vector3(direction.x, 0, direction.y).normalized;
            if (Utils.IsInCamera(character.transform.position + (new Vector3(direction.x, 0, direction.y) * size * 0.5f), Mathf.Abs(character.transform.position.y - Camera.main.transform.position.y)))
            {
                character.Move(direction);
            }
        }

        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.AllMovement))
        {
            dashDirection = Vector3.zero;
        }

        //rotation stick droit
        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.RotateAbsolute) && character.GetScale() * character.PersonalScale > 0 && dashing == 0)
        {
            character.transform.eulerAngles = new Vector3(0, val.x, 0);
            lastMovement = character.transform.position + character.transform.forward;
            didRotation = true;
        }

        if (typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Dash) && character.GetScale() * character.PersonalScale > 0 && dashDirection != Vector3.zero)
        {
            dashing = ((Player)character).DistanceDash / 100;
            AkSoundEngine.PostEvent("S_Dash", character.gameObject);
        }

        if(typeAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Shoot) && character.GetScale() * character.PersonalScale > 0)
        {
            NextState();
        }

        //Mouvement de la souris
        if (typeAct.Equals(BaseInput.TypeAction.Mouse) && acts.Equals(BaseInput.Actions.Rotate) && character.GetScale() * character.PersonalScale > 0 && dashing == 0f)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(val.x, val.y, Mathf.Abs(character.transform.position.y - Camera.main.transform.position.y)));
            lastMovement = worldPosition;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(character.transform.position);
            Vector2 mousePos = new Vector2();
            mousePos.x = val.x - objectPos.x;
            mousePos.y = val.y - objectPos.y;

            float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
            character.transform.localEulerAngles = new Vector3(0, angle, 0);
            didRotation = true;
        }

        //SI aucune rotation n'a été faite , regarder la derniere position connu
        if (!didRotation && lastMovement != Vector3.zero)
        {
            character.transform.LookAt(lastMovement);
        }
        
    }

    public override void UpdateState()
    {
        ((Player)character).UpdateHook();
        //On passe a travers les projectils tant que l'on est en dash
        character.GetComponent<BoxCollider>().enabled = (dashing <= 0);
        if (dashing > 0)
        {
            dashing -= Time.deltaTime;
            dashing = Mathf.Max(0, dashing);
            if (Utils.IsInCamera(character.transform.position + (dashDirection * ((Player)character).dashSpeed * character.PersonalScale * character.GetScale() * Time.deltaTime), Mathf.Abs(character.transform.position.y - Camera.main.transform.position.y)))
            {
                character.Move(dashDirection * ((Player)character).dashSpeed);
            }
            
            if (dashing == 0)
            {
                dashDirection = Vector3.zero;
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
