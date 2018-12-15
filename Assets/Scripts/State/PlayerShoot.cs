using UnityEngine;
using System.Collections;

public class PlayerShoot : State
{

    public Hook hook;

    public PlayerShoot(Character character) : base(character)
    {
        hook = character.GetComponent<Hook>();
    }

    public override void EndState()
    {
        //Unhook
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        // If player is holding
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.Shoot))
        {

        }

        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            hook.ReturnHook();
            NextState();
        }
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character));
    }

    public override void StartState()
    {
        //hook
    }

    public override void UpdateState()
    {
        hook.ShootHook();
        if (hook.currentDistance >= hook.maxDistance)
        {
            hook.ReturnHook();
            NextState();
        }
    }
}
