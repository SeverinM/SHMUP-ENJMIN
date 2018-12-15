using UnityEngine;
using System.Collections;

public abstract class State
{

    public abstract void EndState();
    public abstract void UpdateState();
    public abstract void StartState();
    public abstract void NextState();
    public abstract void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val);
}
