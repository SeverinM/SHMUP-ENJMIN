using UnityEngine;
using System.Collections;

public class FreezeMovement : State
{

    private float duration;

    private float lastTime;

    public FreezeMovement(Character character) : base(character)
    {
        duration = character.scaleDuration;
        character.PersonalScale = 0;
    }

    public override void EndState()
    {
        character.GetComponent<Collider>().enabled = true;
        character.PersonalScale = 1;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        
    }

    public override void StartState()
    {
        character.Life -= 1;

        if (character.Life > 0)
        {
            character.StartRecovery(duration);
        }
        
        character.GetComponent<Collider>().enabled = false;
        
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void UpdateState()
    {
        lastTime += Time.deltaTime;
       
        if (lastTime > duration)
        {
            EndState();
        }

    }
}

