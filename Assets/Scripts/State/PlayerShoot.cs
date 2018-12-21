using UnityEngine;
using System.Collections;

/// <summary>
/// Cet etat a lieu lorsque le joueur lance le hook mais qu'il n'a pas encore touché quelque chose ou que la distance max n'a pas encore été atteinte
/// C'est le seul etat où le hook a une existence physique
/// </summary>
public class PlayerShoot : State
{
    public Transform hook;
    Vector3 originRelative;
    float maxDistance;
    float speedTravel;
    LineRenderer line;

    public PlayerShoot(Character character) : base(character)
    {
        hook = character.Context.ValuesOrDefault<Transform>("Hook", character.transform);
        speedTravel = character.Context.ValuesOrDefault<float>("SpeedHook", 0.7f);
        maxDistance = character.Context.ValuesOrDefault<float>("RangeHook", 10);
        line = hook.GetComponent<LineRenderer>();
    }

    public override void EndState()
    {
        hook.GetComponent<BoxCollider>().enabled = false;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            hook.transform.localPosition = originRelative;
            line.SetPosition(0, hook.transform.position);
            line.SetPosition(1, hook.transform.position);
            character.SetState(new PlayerMovement(character));
        }
    }

    public override void NextState()
    {
        character.Context.SetInDictionary("Origin", originRelative);
        character.SetState(new PlayerMovementDuringHook(character));
    }

    public override void StartState()
    {
        originRelative = hook.transform.localPosition;
        hook.GetComponent<BoxCollider>().enabled = true;
    }

    public override void UpdateState()
    {
        hook.transform.Translate(hook.forward * speedTravel * character.GetScale(), Space.World);
        
        line.SetPosition(0, hook.transform.position);
        line.SetPosition(1, character.transform.position);
        
        if (Vector3.Distance(character.transform.position,hook.transform.position) >= maxDistance)
        {
            hook.transform.localPosition = originRelative;

            line.SetPosition(0, hook.transform.position);
            line.SetPosition(1, hook.transform.position);
            character.SetState(new PlayerMovement(character));
        }
    }
}
