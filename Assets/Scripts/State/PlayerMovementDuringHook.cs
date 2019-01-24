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
    float range;
    Vector3 pos;
    

    public PlayerMovementDuringHook(Character chara) : base(chara)
    {
        coeff = character.Context.ValuesOrDefault<float>("CoeffHook", 0.1f);
        range = character.Context.ValuesOrDefault<float>("RangeHook", 10);
        pos = character.Context.ValuesOrDefault<Vector3>("PositionLand", Vector3.zero);
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
        character.Context.Remove("PostionLand");
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Le joueur va beaucoup moins vite que dans PlayerMouvement
        val *= (coeff * character.GetScale());
        if (typeAct.Equals(BaseInput.TypeAction.Pressed) && acts.Equals(BaseInput.Actions.AllMovement))
        {
            direction.Set(val.x, val.y);
            
            Vector3 projectedPosition = (new Vector3(val.x, 0, val.y) * character.MoveSpeed * Time.deltaTime) + character.transform.position;
            if (Vector3.Distance(projectedPosition , pos) <= range && Utils.IsInCamera(projectedPosition,Mathf.Abs(projectedPosition.y - Camera.main.transform.position.y)))
            {
                character.Move(direction);
            }

            //On detache de force
            if (Vector3.Distance(projectedPosition, pos) > range + 1)
            {
                player.Target.parent.GetComponent<Character>().PersonalScale = 1;   
                character.SetState(new PlayerMovement(character));
            }
        }

        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            //Bouton relaché , on passe au winch
            NextState();
        }
    }

    public override string GetName()
    {
        return "PlayerMovementDuringHook";
    }
}
