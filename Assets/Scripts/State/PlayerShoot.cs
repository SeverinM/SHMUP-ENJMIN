using UnityEngine;
using System.Collections;

/// <summary>
/// Cet etat a lieu lorsque le joueur lance le hook mais qu'il n'a pas encore touché quelque chose ou que la distance max n'a pas encore été atteinte
/// C'est le seul etat où le hook a une existence physique
/// </summary>
public class PlayerShoot : State
{
    private Player player;
    float maxDistance;
    float speedTravel;

    public PlayerShoot(Character character) : base(character)
    {
        player = character.GetComponent<Player>();
        speedTravel = character.Context.ValuesOrDefault<float>("SpeedHook", 0.7f);
        maxDistance = character.Context.ValuesOrDefault<float>("RangeHook", 10);
    }

    public override void EndState()
    {
        player.Hook.GetComponent<BoxCollider>().enabled = false;
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        if (typeAct.Equals(BaseInput.TypeAction.Up) && acts.Equals(BaseInput.Actions.Shoot))
        {
            // Si le joueur relache le bouton, alors on remet le hook à la position de départ
            player.ResetHook();
            character.SetState(new PlayerMovement(character));
        }
    }

    public override void NextState()
    {
        character.SetState(new PlayerMovementDuringHook(character));
    }

    public override void StartState()
    {
        player.Hook.GetComponent<BoxCollider>().enabled = true;
        AkSoundEngine.PostEvent("H_Launch", character.gameObject);
    }

    public override void UpdateState()
    {
        // Déplacement du grappin 
        player.Hook.transform.Translate(player.Hook.forward * speedTravel * character.GetScale() * Time.deltaTime, Space.World);
       

        // Si le grapin dépasse la distance maximale, le joueur revient à l'état PlayerMovement, et le grappin est retracté
        if (Vector3.Distance(character.transform.position, player.Hook.transform.position) >= maxDistance)
        {
            player.ResetHook();

            character.SetState(new PlayerMovement(character));
        }
    }

    public override string GetName()
    {
        return "PlayerShoot";
    }
}
