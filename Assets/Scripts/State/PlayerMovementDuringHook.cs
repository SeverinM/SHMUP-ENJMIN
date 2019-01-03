using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cet etat a lieu quand lorsque le grappin a touché quelque chose mais que le joueur n'a pas encore relaché la touche , il ressemble beaucoup a PlayerMouvement
/// </summary>
public class PlayerMovementDuringHook : PlayerMovement {

    //La positon du grappin relativement a son père
    Transform hook;
    float coeff;
    Player player;
    

    public PlayerMovementDuringHook(Character chara) : base(chara)
    {
        coeff = character.Context.ValuesOrDefault<float>("CoeffHook", 0.1f);
        player = (Player)chara;
    }

    public override void StartState()
    {
        player.Target.parent.GetComponent<Character>().PersonalScale = 0;
    }

    public override void NextState()
    {
        character.SetState(new PlayerWinch(character));
    }

    public override void UpdateState()
    {
        //Le vaisseau fera toujours face au hook
        character.transform.forward = (player.target.position - character.transform.position).normalized;
        player.Hook.transform.position = player.Target.transform.position;
    }

    public override void EndState()
    {
       
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Le joueur va beaucoup moins vite que dans PlayerMouvement
        val *= (coeff * character.GetScale());
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement))
        {
            direction.Set(val.x, val.y);
            character.Move(direction);
        }

        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            //Bouton relaché , on passe au winch
            NextState();
        }
    }
}
