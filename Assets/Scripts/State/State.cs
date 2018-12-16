using UnityEngine;
using System.Collections;

public abstract class State
{
    protected Character character;

    public State(Character character)
    {
        this.character = character;
    }

    public virtual void EndState() { }
    public virtual void UpdateState() { }
    public virtual void StartState() { }
    public virtual void NextState() { }
    public virtual void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val) { }
}
