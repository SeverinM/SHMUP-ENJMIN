using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Etat servant juste dans les transitions entre niveaux 
/// </summary>
public class IdleTransition : State {

    public IdleTransition(Character chara) : base(chara){ }
    Vector3 deltaPosition;

    public override string GetName()
    {
        return "IdleTransition";
    }

    public override void StartState()
    {
        deltaPosition = Camera.main.transform.position - character.transform.position;
        character.GetComponent<Animator>().SetTrigger("Start");
        Utils.StartFading(2f, Color.black, () => { }, () => { });
    }

    public override void EndState()
    {
        ((Player)character).RaiseNextLevel();
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character));
    }

    public override void UpdateState()
    {
        Camera.main.transform.position = character.transform.position + deltaPosition;
    }
}
