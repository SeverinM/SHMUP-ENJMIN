using UnityEngine;
using System.Collections;

public class PlayerShoot : State
{

    public Transform hook;
    Vector3 originRelative;
    float maxDistance = 10;
    float speedTravel = 1f;

    public PlayerShoot(Character character) : base(character)
    {
        hook = character.transform.GetChild(0);
    }

    public override void EndState()
    {
        originRelative = hook.transform.position - character.transform.position;
        hook.GetComponent<BoxCollider>().enabled = false;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            hook.transform.position = character.transform.position + originRelative;
            character.SetState(new PlayerMovement(character));
        }
    }

    public override void NextState()
    {
        character.SetState(new PlayerWinch(character, hook, originRelative));
    }

    public override void StartState()
    {
        originRelative = hook.position - character.transform.position;
        hook.GetComponent<BoxCollider>().enabled = true;
    }

    public override void UpdateState()
    {
        hook.transform.Translate(originRelative * speedTravel,Space.World);
        
        if (Vector3.Distance(character.transform.position,hook.transform.position) >= maxDistance)
        {
            hook.transform.position = hook.parent.position + originRelative;
            character.SetState(new PlayerMovement(character));
        }
    }
}
