using UnityEngine;
using System.Collections;

public class PlayerWinch : State
{
    Transform hook;
    Vector3 origin;
    float duration = 0.5f;
    float actualTime = 0;
    Vector3 positionRelative;
    LineRenderer line;

    public PlayerWinch(Character character, Transform hook, Vector3 pos) : base(character)
    {
        this.hook = hook;
        line = hook.GetComponent<LineRenderer>();
        positionRelative = pos;
    }

    public override void EndState()
    {
        hook.transform.position = character.transform.position + positionRelative;
        line.SetPosition(0, hook.position);
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character));
    }

    public override void StartState()
    {
        origin = character.transform.position;
    }

    public override void UpdateState()
    {       
        Vector3 copy = hook.transform.position;
        actualTime += Time.deltaTime;

        character.transform.position = Vector3.Lerp(origin, hook.transform.position, actualTime / duration);

        line.SetPosition(0, hook.position);
        line.SetPosition(1, character.transform.position);

        float distanceToHook = Vector3.Distance(character.transform.position, hook.transform.position);
        hook.transform.position = copy;

        if (distanceToHook < 0.5)
        {
            hook.transform.position = hook.transform.parent.position;
            line.SetPosition(0, hook.position);
            NextState();
        }
    }
}
