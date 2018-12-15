using UnityEngine;
using System.Collections;

public class PlayerWinch : State
{
    Transform hook;
    Vector3 origin;
    float duration = 0.1f;
    float actualTime = 0;
    Vector3 positionRelative;

    public PlayerWinch(Character character, Transform hook, Vector3 pos) : base(character)
    {
        this.hook = hook;
        positionRelative = pos;
    }

    public override void EndState()
    {
        hook.transform.position = character.transform.position + positionRelative;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
      
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

        float distanceToHook = Vector3.Distance(character.transform.position, hook.transform.position);
        hook.transform.position = copy;

        if (distanceToHook < 0.1)
        {
            hook.transform.position = hook.transform.parent.position;
            NextState();
        }
    }
}
