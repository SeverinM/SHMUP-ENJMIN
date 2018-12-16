using UnityEngine;
using System.Collections;

public class EnemyAttack : State
{
    public EnemyAttack(Character character) : base(character)
    {
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        base.NextState();
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }


}
