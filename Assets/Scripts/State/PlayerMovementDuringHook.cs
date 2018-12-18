using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cet etat a lieu quand lorsque le grappin a touché quelque chose mais que le joueur n'a pas encore relaché la touche , il ressemble beaucoup a PlayerMouvement
/// </summary>
public class PlayerMovementDuringHook : PlayerMovement {

    //La positon du grappin relativement a son père
    Vector3 originRelative;
    Transform hook;
    Context cont;

    public PlayerMovementDuringHook(Character chara,Context ctx) : base(chara,ctx)
    {
        cont = ctx;
        originRelative = ctx.ValuesOrDefault<Vector3>("Origin", Vector3.forward);
        hook = ctx.ValuesOrDefault<Transform>("Hook", character.transform);
        //Le hook perd temporairement son statut d'enfant juste pour cet etat
        hook.parent = null;
    }

    public override void NextState()
    {
        character.SetState(new PlayerWinch(character,cont));
    }

    public override void UpdateState()
    {
        //Le vaisseau fera toujours face au hook
        character.transform.forward = (hook.transform.position - character.transform.position).normalized;
    }

    public override void EndState()
    {
        //Reconstitution de la relation pere / enfant
        hook.parent = character.transform;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        //Le joueur va beaucoup moins vite que dans PlayerMouvement
        val /= 10;
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

        //On actualise la position de la ligne reliant le hook et le vaisseau
        hook.GetComponent<LineRenderer>().SetPosition(1, character.transform.position);
    }
}
