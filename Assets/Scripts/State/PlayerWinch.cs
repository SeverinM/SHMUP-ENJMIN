using UnityEngine;
using System.Collections;

public class PlayerWinch : State
{
    Transform hook;
    Transform shield;
    Transform target;
    
    //Position du vaisseau AVANT d'etre tracté
    Vector3 origin;

    //Vitesse de traversé du hook
    float speedTravel;

    //La position du hook relative au vaisseau avant qu'il soit tiré
    Vector3 positionRelative;

    //Ligne creant le lien entre vaisseau et hook
    LineRenderer line;
    Vector3 copy;

    public enum HookMode
    {
        Pull,
        Winch
    }
    HookMode currentMode;

    public PlayerWinch(Character character) : base(character)
    {
        hook = character.Context.ValuesOrDefault<Transform>("Hook", character.transform);
        shield = character.Context.ValuesOrDefault<Transform>("Shield", character.transform);
        line = hook.GetComponent<LineRenderer>();
        positionRelative = character.Context.ValuesOrDefault<Vector3>("Origin", Vector3.forward);
        speedTravel = character.Context.ValuesOrDefault<float>("SpeedWinch", 10);
        currentMode = character.Context.ValuesOrDefault<HookMode>("HookMode", HookMode.Winch);
        target = character.Context.ValuesOrDefault<Transform>("Target", target);
    }

    public override void EndState()
    {
        line.SetPosition(0, hook.position);
        shield.GetComponent<Shield>().IsWinching = false;
        character.Context.Remove("Target");
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character));
    }

    public override void StartState()
    {
        origin = character.transform.position;
        shield.GetComponent<Shield>().IsWinching = true;
    }

    public override void UpdateState()
    {
        if (currentMode == HookMode.Winch)
        {
            copy = hook.transform.position;
            character.transform.position += character.transform.forward * Time.deltaTime * character.GetScale() * speedTravel;
            hook.transform.position = copy;
        }

        if (currentMode == HookMode.Pull)
        {
            // Pull the parent because we set a pullable tag to a child
            target.parent.transform.position -= character.transform.forward * Time.deltaTime * character.GetScale() * speedTravel;
            hook.transform.position -= character.transform.forward * Time.deltaTime * character.GetScale() * speedTravel;
        }

        line.SetPosition(0, hook.position);
        line.SetPosition(1, character.transform.position);

        float distanceToHook = Vector3.Distance(character.transform.position, hook.transform.position);

        //Si la distance hook / vaisseau est inferieur a celle d'origine retourner a l'etat Idle
        if (distanceToHook <= positionRelative.magnitude)
        {
            hook.transform.localPosition = positionRelative;
            line.SetPosition(0, hook.position);
            line.SetPosition(1, hook.position);
            NextState();
        }
    }
}
