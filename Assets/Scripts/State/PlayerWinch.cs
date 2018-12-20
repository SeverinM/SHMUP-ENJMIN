using UnityEngine;
using System.Collections;

public class PlayerWinch : State
{
    Transform hook;
    Transform shield;
    
    //Position du vaisseau AVANT d'etre tracté
    Vector3 origin;

    //Vitesse de traversé du hook
    float speedTravel = 10;

    //La position du hook relative au vaisseau avant qu'il soit tiré
    Vector3 positionRelative;

    //Ligne creant le lien entre vaisseau et hook
    LineRenderer line;

    Context cont;
    Vector3 copy;

    public PlayerWinch(Character character,Context ctx) : base(character)
    {
        cont = ctx;
        hook = ctx.ValuesOrDefault<Transform>("Hook", character.transform);
        shield = ctx.ValuesOrDefault<Transform>("Shield", character.transform);
        line = hook.GetComponent<LineRenderer>();
        positionRelative = ctx.ValuesOrDefault<Vector3>("Origin", Vector3.forward);
    }

    public override void EndState()
    {
        line.SetPosition(0, hook.position);
        shield.GetComponent<Shield>().IsWinching = false;
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character,cont));
    }

    public override void StartState()
    {
        origin = character.transform.position;
        shield.GetComponent<Shield>().IsWinching = true;
    }

    public override void UpdateState()
    {       
        copy = hook.transform.position;

        character.transform.position += character.transform.forward * Time.deltaTime * Constants.TimeScalePlayer * speedTravel;

        line.SetPosition(0, hook.position);
        line.SetPosition(1, character.transform.position);

        float distanceToHook = Vector3.Distance(character.transform.position, hook.transform.position);
        hook.transform.position = copy;

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
