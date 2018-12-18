using UnityEngine;
using System.Collections;

public class PlayerWinch : State
{
    Transform hook;
    Transform shield;
    
    //Position du vaisseau AVANT d'etre tracté
    Vector3 origin;

    //En combien de temps le vaisseau va rejoindre le hook 
    float duration = 0.5f;
    float actualTime = 0;

    //La position du hook relative au vaisseau avant qu'il soit tiré
    Vector3 positionRelative;

    //Ligne creant le lien entre vaisseau et hook
    LineRenderer line;

    public PlayerWinch(Character character, Transform hook, Vector3 pos) : base(character)
    {
        this.hook = hook;
        shield = ((Player)character).Shield;
        line = hook.GetComponent<LineRenderer>();
        positionRelative = pos;
    }

    public override void EndState()
    {
        line.SetPosition(0, hook.position);
        shield.GetComponent<BoxCollider>().enabled = false;
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character));
    }

    public override void StartState()
    {
        origin = character.transform.position;
        shield.GetComponent<BoxCollider>().enabled = true;
    }

    public override void UpdateState()
    {       
        Vector3 copy = hook.transform.position;
        actualTime += Time.deltaTime * Constants.TimeScalePlayer;

        character.transform.position = Vector3.Lerp(origin, hook.transform.position, actualTime / duration);

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
