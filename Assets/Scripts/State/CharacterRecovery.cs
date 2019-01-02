using UnityEngine;
using System.Collections;


/// <summary>
/// Pendant un cours instant, change l'aspect du personnage et lui active sa protection 
/// </summary>
/// <param name="duration"></param>
/// <returns></returns>
public class CharacterRecovery : State
{
    private float duration;
    Material orignal;
    State movement;

    private float lastTime;

    public CharacterRecovery(Character character, State movement, float duration) : base(character)
    {
        this.duration = duration;
        orignal = character.GetComponent<MeshRenderer>().material;
        this.movement = movement;

        if(movement == null)
        {
            movement = new FollowPathMovement(character, null, true);
        }
    }

    public override void EndState()
    {
        character.GetComponent<MeshRenderer>().material = orignal;
        character.protection.SetActive(false);
        character.GetComponent<Collider>().enabled = true;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        movement.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        character.SetState(movement);
    }

    public override void StartState()
    {
        if (character.recoveryMat != null)
        {
            character.GetComponent<MeshRenderer>().material = character.recoveryMat;
        }

        character.GetComponent<Collider>().enabled = false;
        character.protection.SetActive(true);

        movement.StartState();
    }

    public override void UpdateState()
    {        
        movement.UpdateState();
        lastTime += Time.deltaTime;

        if (lastTime > duration)
        {
            NextState();
        }
    }

}
