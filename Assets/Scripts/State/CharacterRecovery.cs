using UnityEngine;
using System.Collections;


/// <summary>
/// Pendant un cours instant, change l'aspect du personnage et lui active sa protection 
/// </summary>
/// <param name="duration"></param>
/// <returns></returns>
public class CharacterRecovery : State
{
    // Duration the player last in this state
    private float duration;
    private float lastTime;

    Material orignal;

    public CharacterRecovery(Character character, float duration) : base(character)
    {
        this.duration = duration;
        orignal = character.GetComponent<MeshRenderer>().material;
    }

    public override void EndState()
    {
        character.GetComponent<MeshRenderer>().material = orignal;
        character.protection.SetActive(false);
        character.GetComponent<Collider>().enabled = true;
    }

    public override void NextState()
    {
        ((Enemy)character).FollowRandomPath();
    }

    public override void StartState()
    {
        if (character.recoveryMat != null)
        {
            character.GetComponent<MeshRenderer>().material = character.recoveryMat;
        }

        character.GetComponent<Collider>().enabled = false;
        character.protection.SetActive(true);
    }

    public override void UpdateState()
    {        
        lastTime += Time.deltaTime;

        if (lastTime > duration)
        {
            NextState();
        }
    }

}
