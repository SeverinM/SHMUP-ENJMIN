using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Etat servant juste dans les transitions entre niveaux 
/// </summary>
public class IdleTransition : State {

    public IdleTransition(Character chara) : base(chara){ }

    public override string GetName()
    {
        return "IdleTransition";
    }

    public override void StartState()
    {
        character.GetComponent<Animator>().SetTrigger("Start");
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovement(character));
    }
}
