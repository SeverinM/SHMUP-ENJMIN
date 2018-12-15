using UnityEngine;
using System.Collections;

public class PlayerHook : State
{
    private GameObject hook;

    public PlayerHook(Character character, GameObject hook) : base(character)
    {
        this.hook = hook;
    }

    public override void EndState()
    {
        
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
      
    }

    public override void UpdateState()
    {
      
        Vector3 copy = hook.transform.position;
        character.transform.position = Vector3.MoveTowards(character.transform.position, hook.transform.position, 10 * Time.deltaTime);

        float distanceToHook = Vector3.Distance(character.transform.position, hook.transform.position);
        hook.transform.position = copy;

        if (distanceToHook < 1)
        {
            NextState();
        }
    }
}
